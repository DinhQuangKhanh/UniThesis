using Microsoft.Extensions.Options;
using UniThesis.Infrastructure.BackgroundJobs;
using UniThesis.Infrastructure.Services.FileStorage;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.API.Common.Security;

/// <summary>
/// Uploads files to quarantine storage and enqueues background malware scan jobs.
/// Context-agnostic — the <see cref="AttachmentScanContext"/> carries folder paths,
/// document type, and metadata so a single workflow handles all upload scenarios.
/// </summary>
internal sealed class AttachmentScanWorkflow : IAttachmentScanWorkflow
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IMalwareScanAuditLogger _auditLogger;
    private readonly IQuarantinedAttachmentRepository _quarantineTracking;
    private readonly MalwareScanOptions _scanOptions;
    private readonly ILogger<AttachmentScanWorkflow> _logger;

    public AttachmentScanWorkflow(
        IFileStorageService fileStorageService,
        IBackgroundJobService backgroundJobService,
        IMalwareScanAuditLogger auditLogger,
        IQuarantinedAttachmentRepository quarantineTracking,
        IOptions<MalwareScanOptions> scanOptions,
        ILogger<AttachmentScanWorkflow> logger)
    {
        _fileStorageService = fileStorageService;
        _backgroundJobService = backgroundJobService;
        _auditLogger = auditLogger;
        _quarantineTracking = quarantineTracking;
        _scanOptions = scanOptions.Value;
        _logger = logger;
    }

    public async Task<AttachmentQueueResult> QueueAsync(
        AttachmentScanContext context,
        IReadOnlyCollection<IFormFile>? attachments,
        CancellationToken cancellationToken = default)
    {
        if (attachments is null || attachments.Count == 0)
        {
            return AttachmentQueueResult.Ok(0);
        }

        var queuedCount = 0;
        var quarantineFolder = $"{context.FolderPrefix}/quarantine/{context.FolderPartitionId:N}/{DateTime.UtcNow:yyyyMMdd}";

        foreach (var file in attachments)
        {
            try
            {
                await using var stream = file.OpenReadStream();
                var uploadResult = await _fileStorageService.UploadAsync(
                    stream,
                    file.FileName,
                    quarantineFolder,
                    cancellationToken);

                if (!uploadResult.Success || string.IsNullOrWhiteSpace(uploadResult.FilePath))
                {
                    var error = uploadResult.Error ?? "Không thể lưu file vào khu vực quarantine.";
                    await _auditLogger.LogAsync(
                        verdict: "Error",
                        file: file,
                        message: error,
                        scannerResponse: null,
                        metadata: BuildMetadata(context, "upload-quarantine"),
                        cancellationToken: cancellationToken);

                    if (_scanOptions.FailClosed)
                    {
                        return AttachmentQueueResult.Failed(
                            "Không thể đưa tệp đính kèm vào hàng đợi quét mã độc. Vui lòng thử lại sau.",
                            queuedCount);
                    }

                    continue;
                }

                var quarantinePath = uploadResult.FilePath;
                var documentTypeInt = (int)context.DocumentType;

                // Track this quarantine entry so QuarantineRetryJob can re-enqueue if stuck
                await _quarantineTracking.AddAsync(new QuarantinedAttachmentDocument
                {
                    FolderPrefix = context.FolderPrefix,
                    ProjectId = context.ProjectId,
                    UploadedBy = context.UploadedBy,
                    DocumentTypeInt = documentTypeInt,
                    FolderPartitionId = context.FolderPartitionId,
                    QuarantinePath = quarantinePath,
                    OriginalFileName = file.FileName,
                }, cancellationToken);

                _backgroundJobService.Enqueue<AttachmentScanJob>(
                    job => job.ExecuteAsync(
                        context.FolderPrefix,
                        context.ProjectId,
                        context.UploadedBy,
                        documentTypeInt,
                        context.FolderPartitionId,
                        quarantinePath,
                        file.FileName));

                queuedCount++;

                await _auditLogger.LogAsync(
                    verdict: "Queued",
                    file: file,
                    message: "Attachment enqueued for asynchronous malware scan.",
                    scannerResponse: null,
                    metadata: BuildMetadata(context, "queue", new Dictionary<string, object?>
                    {
                        ["quarantinePath"] = quarantinePath,
                    }),
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue malware scan for {FileName}", file.FileName);

                await _auditLogger.LogAsync(
                    verdict: "Error",
                    file: file,
                    message: "Exception while queueing asynchronous malware scan job.",
                    scannerResponse: ex.Message,
                    metadata: BuildMetadata(context, "queue-exception"),
                    cancellationToken: cancellationToken);

                if (_scanOptions.FailClosed)
                {
                    return AttachmentQueueResult.Failed(
                        "Không thể đưa tệp đính kèm vào hàng đợi quét mã độc. Vui lòng thử lại sau.",
                        queuedCount);
                }
            }
        }

        return queuedCount == 0
            ? AttachmentQueueResult.Failed(
                "Không thể đưa tệp đính kèm vào hàng đợi quét mã độc. Vui lòng kiểm tra cấu hình Firebase Storage hoặc xem log audit.",
                queuedCount)
            : AttachmentQueueResult.Ok(queuedCount);
    }

    private static Dictionary<string, object?> BuildMetadata(
        AttachmentScanContext context,
        string stage,
        IDictionary<string, object?>? extra = null)
    {
        var metadata = new Dictionary<string, object?>
        {
            ["projectId"] = context.ProjectId,
            ["uploadedBy"] = context.UploadedBy,
            ["category"] = context.FolderPrefix,
            ["stage"] = stage
        };

        if (context.ExtraMetadata is not null)
        {
            foreach (var kv in context.ExtraMetadata)
                metadata[kv.Key] = kv.Value;
        }

        if (extra is not null)
        {
            foreach (var kv in extra)
                metadata[kv.Key] = kv.Value;
        }

        return metadata;
    }
}
