using Microsoft.Extensions.Options;
using UniThesis.Infrastructure.BackgroundJobs;
using UniThesis.Infrastructure.Services.FileStorage;

namespace UniThesis.API.Common.Security;

internal sealed class TopicProposalAttachmentScanWorkflow : ITopicProposalAttachmentScanWorkflow
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IMalwareScanAuditLogger _auditLogger;
    private readonly MalwareScanOptions _scanOptions;
    private readonly ILogger<TopicProposalAttachmentScanWorkflow> _logger;

    public TopicProposalAttachmentScanWorkflow(
        IFileStorageService fileStorageService,
        IBackgroundJobService backgroundJobService,
        IMalwareScanAuditLogger auditLogger,
        IOptions<MalwareScanOptions> scanOptions,
        ILogger<TopicProposalAttachmentScanWorkflow> logger)
    {
        _fileStorageService = fileStorageService;
        _backgroundJobService = backgroundJobService;
        _auditLogger = auditLogger;
        _scanOptions = scanOptions.Value;
        _logger = logger;
    }

    public async Task<TopicProposalAttachmentQueueResult> QueueAsync(
        Guid projectId,
        Guid poolId,
        Guid uploadedBy,
        IReadOnlyCollection<IFormFile>? attachments,
        CancellationToken cancellationToken = default)
    {
        if (attachments is null || attachments.Count == 0)
        {
            return TopicProposalAttachmentQueueResult.Ok(0);
        }

        var queuedCount = 0;
        var quarantineFolder = $"topic-proposals/quarantine/{poolId:N}/{DateTime.UtcNow:yyyyMMdd}";

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
                        metadata: new Dictionary<string, object?>
                        {
                            ["poolId"] = poolId,
                            ["stage"] = "upload-quarantine"
                        },
                        cancellationToken: cancellationToken);

                    if (_scanOptions.FailClosed)
                    {
                        return TopicProposalAttachmentQueueResult.Failed(
                            "Không thể đưa tệp đính kèm vào hàng đợi quét mã độc. Vui lòng thử lại sau.",
                            queuedCount);
                    }

                    continue;
                }

                var quarantinePath = uploadResult.FilePath;
                var jobId = _backgroundJobService.Enqueue<TopicProposalAttachmentScanJob>(
                    job => job.ExecuteAsync(projectId, poolId, uploadedBy, quarantinePath, file.FileName));

                queuedCount++;

                await _auditLogger.LogAsync(
                    verdict: "Queued",
                    file: file,
                    message: "Attachment enqueued for asynchronous malware scan.",
                    scannerResponse: null,
                    metadata: new Dictionary<string, object?>
                    {
                        ["projectId"] = projectId,
                        ["poolId"] = poolId,
                        ["uploadedBy"] = uploadedBy,
                        ["jobId"] = jobId,
                        ["quarantinePath"] = quarantinePath,
                        ["stage"] = "queue"
                    },
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue malware scan for attachment {FileName}", file.FileName);

                await _auditLogger.LogAsync(
                    verdict: "Error",
                    file: file,
                    message: "Exception while queueing asynchronous malware scan job.",
                    scannerResponse: ex.Message,
                    metadata: new Dictionary<string, object?>
                    {
                        ["projectId"] = projectId,
                        ["poolId"] = poolId,
                        ["uploadedBy"] = uploadedBy,
                        ["stage"] = "queue-exception"
                    },
                    cancellationToken: cancellationToken);

                if (_scanOptions.FailClosed)
                {
                    return TopicProposalAttachmentQueueResult.Failed(
                        "Không thể đưa tệp đính kèm vào hàng đợi quét mã độc. Vui lòng thử lại sau.",
                        queuedCount);
                }
            }
        }

        if (queuedCount == 0)
        {
            return TopicProposalAttachmentQueueResult.Failed(
                "Không thể đưa tệp đính kèm vào hàng đợi quét mã độc. Vui lòng kiểm tra cấu hình Firebase Storage hoặc xem log audit.",
                queuedCount);
        }

        return TopicProposalAttachmentQueueResult.Ok(queuedCount);
    }
}
