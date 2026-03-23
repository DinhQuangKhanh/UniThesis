using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Enums.Document;
using UniThesis.Domain.Enums.Notification;
using UniThesis.Infrastructure.Services.FileStorage;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.API.Common.Security;

/// <summary>
/// Background job: download from quarantine → malware scan → promote to clean folder → create Document entity.
/// Parameterised by folder prefix + DocumentType so a single class handles all upload contexts.
/// </summary>
internal sealed class AttachmentScanJob
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IProjectRepository _projectRepository;
    private readonly IMalwareScanner _malwareScanner;
    private readonly IMalwareScanAuditLogger _auditLogger;
    private readonly INotificationService _notificationService;
    private readonly IQuarantinedAttachmentRepository _quarantineTracking;
    private readonly ILogger<AttachmentScanJob> _logger;

    public AttachmentScanJob(
        IFileStorageService fileStorageService,
        IProjectRepository projectRepository,
        IMalwareScanner malwareScanner,
        IMalwareScanAuditLogger auditLogger,
        INotificationService notificationService,
        IQuarantinedAttachmentRepository quarantineTracking,
        ILogger<AttachmentScanJob> logger)
    {
        _fileStorageService = fileStorageService;
        _projectRepository = projectRepository;
        _malwareScanner = malwareScanner;
        _auditLogger = auditLogger;
        _notificationService = notificationService;
        _quarantineTracking = quarantineTracking;
        _logger = logger;
    }

    /// <param name="folderPrefix">Storage category, e.g. "topic-proposals" or "topic-documents".</param>
    /// <param name="documentTypeInt">Serialised <see cref="DocumentType"/> enum value.</param>
    /// <param name="folderPartitionId">ID used to partition clean storage (poolId for proposals, projectId for documents).</param>
    public async Task ExecuteAsync(
        string folderPrefix,
        Guid projectId,
        Guid uploadedBy,
        int documentTypeInt,
        Guid folderPartitionId,
        string quarantinePath,
        string originalFileName)
    {
        var documentType = (DocumentType)documentTypeInt;
        var metadata = new Dictionary<string, object?>
        {
            ["projectId"] = projectId,
            ["uploadedBy"] = uploadedBy,
            ["category"] = folderPrefix,
            ["quarantinePath"] = quarantinePath,
            ["stage"] = "background-scan"
        };

        try
        {
            await using var downloaded = await _fileStorageService.DownloadAsync(quarantinePath);
            if (downloaded is null)
            {
                // Quarantine file is gone — check if it was already promoted to clean storage.
                // This happens when Hangfire retries after the file was moved but Document save failed.
                var tracking = await _quarantineTracking.GetByQuarantinePathAsync(quarantinePath);
                if (tracking?.CleanPath is { Length: > 0 } cleanPath)
                {
                    _logger.LogInformation(
                        "Quarantine file already promoted. Recovering Document entity from CleanPath: {CleanPath}",
                        cleanPath);

                    await _auditLogger.LogAsync("Recovery", null,
                        "Recovering Document entity from previously promoted clean file.",
                        null, metadata: new Dictionary<string, object?>(metadata) { ["cleanPath"] = cleanPath, ["stage"] = "recovery" });

                    var saved = await InsertDocumentAsync(
                        projectId, uploadedBy, documentType, cleanPath, originalFileName, 0, metadata);

                    if (saved)
                    {
                        await _quarantineTracking.DeleteByQuarantinePathAsync(quarantinePath);
                        await _notificationService.SendAsync(
                            userId: uploadedBy,
                            title: "Tải lên tài liệu thành công",
                            content: $"File '{originalFileName}' đã được quét sạch và lưu vào hệ thống.",
                            type: NotificationType.Success,
                            category: NotificationCategory.Project);
                    }
                    return;
                }

                await _auditLogger.LogAsync("Error", null,
                    "Quarantine file not found and no CleanPath recovery available.",
                    null, metadata: metadata);
                await _quarantineTracking.DeleteByQuarantinePathAsync(quarantinePath);
                await NotifyFailureAsync(uploadedBy, originalFileName,
                    $"File '{originalFileName}' không tìm thấy trong khu vực quarantine.");
                return;
            }

            Stream scanStream = downloaded;
            if (!downloaded.CanSeek)
            {
                var mem = new MemoryStream();
                await downloaded.CopyToAsync(mem);
                mem.Position = 0;
                downloaded.Dispose();
                scanStream = mem;
            }
            else
            {
                downloaded.Position = 0;
            }

            await ProcessStreamAsync(
                folderPrefix, projectId, uploadedBy, documentType,
                folderPartitionId, quarantinePath, originalFileName,
                scanStream, metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Background malware scan job failed for {QuarantinePath}", quarantinePath);

            await _auditLogger.LogAsync("Error", null,
                "Background malware scan job failed with unhandled exception.",
                ex.ToString(),
                metadata: new Dictionary<string, object?>(metadata) { ["stage"] = "unhandled-exception" });

            await NotifyFailureAsync(uploadedBy, originalFileName,
                $"File '{originalFileName}' gặp lỗi trong quá trình xử lý. Hệ thống sẽ tự động thử lại.");

            throw; // Rethrow so Hangfire retries
        }
    }

    private async Task ProcessStreamAsync(
        string folderPrefix,
        Guid projectId,
        Guid uploadedBy,
        DocumentType documentType,
        Guid folderPartitionId,
        string quarantinePath,
        string originalFileName,
        Stream stream,
        IDictionary<string, object?> metadata)
    {
        var length = stream.CanSeek ? stream.Length : 0;
        var formFile = new FormFile(stream, 0, length, "attachments", originalFileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };

        // ── Step 1: Malware scan ──
        var scanResult = await _malwareScanner.ScanAsync([formFile]);

        if (scanResult.ScannerUnavailable)
        {
            await _auditLogger.LogAsync("Warning", formFile,
                $"Malware scanner unavailable: {scanResult.Message}. File kept in quarantine for retry.",
                null, metadata: new Dictionary<string, object?>(metadata) { ["stage"] = "scan-unavailable" });

            // Rethrow so Hangfire retries later when scanner might be back
            throw new InvalidOperationException(
                $"Malware scanner unavailable for '{originalFileName}': {scanResult.Message}");
        }

        if (!scanResult.IsClean)
        {
            await _auditLogger.LogAsync("Quarantined", formFile,
                $"Attachment blocked: malware detected — {scanResult.Message}",
                null, metadata: new Dictionary<string, object?>(metadata) { ["stage"] = "scan-infected" });
            await _quarantineTracking.DeleteByQuarantinePathAsync(quarantinePath);
            await NotifyFailureAsync(uploadedBy, originalFileName,
                $"File '{originalFileName}' bị phát hiện chứa mã độc và đã bị giữ lại.");
            return;
        }

        await _auditLogger.LogAsync("ScanClean", formFile,
            "Attachment passed malware scan.",
            null, metadata: new Dictionary<string, object?>(metadata) { ["stage"] = "scan-clean" });

        // ── Step 2: Promote from quarantine to clean storage ──
        var cleanFolder = $"{folderPrefix}/clean/{folderPartitionId:N}/{DateTime.UtcNow:yyyyMMdd}";
        var moveResult = await _fileStorageService.MoveAsync(quarantinePath, cleanFolder);
        if (!moveResult.Success || string.IsNullOrWhiteSpace(moveResult.FilePath))
        {
            await _auditLogger.LogAsync("Error", formFile,
                "Failed to promote clean attachment from quarantine to clean storage.",
                moveResult.Error,
                metadata: new Dictionary<string, object?>(metadata) { ["stage"] = "promote-failed" });

            // Transient storage failure — rethrow so Hangfire retries.
            // Notification sent by outer catch in ExecuteAsync.
            throw new InvalidOperationException(
                $"Failed to promote '{originalFileName}' from quarantine: {moveResult.Error}");
        }

        var cleanPath = moveResult.FilePath;

        await _auditLogger.LogAsync("Promoted", formFile,
            "Attachment promoted from quarantine to clean storage.",
            null,
            metadata: new Dictionary<string, object?>(metadata)
            {
                ["cleanPath"] = cleanPath,
                ["stage"] = "promoted"
            });

        // Record clean path BEFORE DB write so that if Hangfire retries after this point,
        // ExecuteAsync can recover from CleanPath instead of re-downloading from quarantine.
        await _quarantineTracking.SetCleanPathAsync(quarantinePath, cleanPath);

        // ── Step 3: Create Document entity ──
        var saved = await InsertDocumentAsync(
            projectId, uploadedBy, documentType, cleanPath, originalFileName, formFile.Length, metadata);

        if (!saved)
        {
            // InsertDocumentAsync already logged + notified.
            // Tracking record stays intact — QuarantineRetryJob will re-attempt.
            return;
        }

        // ── Step 4: Cleanup + success notification ──
        await _quarantineTracking.DeleteByQuarantinePathAsync(quarantinePath);

        await _auditLogger.LogAsync("Completed", formFile,
            "Attachment scan pipeline completed successfully.",
            null,
            metadata: new Dictionary<string, object?>(metadata)
            {
                ["cleanPath"] = cleanPath,
                ["stage"] = "completed"
            });

        await _notificationService.SendAsync(
            userId: uploadedBy,
            title: "Tải lên tài liệu thành công",
            content: $"File '{originalFileName}' đã được quét sạch và lưu vào hệ thống.",
            type: NotificationType.Success,
            category: NotificationCategory.Project);
    }

    /// <summary>
    /// Inserts a Document row directly via raw SQL (bypasses EF change tracker + interceptors).
    /// This eliminates DbUpdateConcurrencyException caused by the aggregate-level save pipeline.
    /// </summary>
    /// <returns><c>true</c> if inserted (or already existed); <c>false</c> on failure.</returns>
    private async Task<bool> InsertDocumentAsync(
        Guid projectId,
        Guid uploadedBy,
        DocumentType documentType,
        string cleanPath,
        string originalFileName,
        long fileSize,
        IDictionary<string, object?> metadata)
    {
        try
        {
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            var inserted = await _projectRepository.InsertDocumentAsync(
                projectId,
                fileName: Path.GetFileName(cleanPath),
                originalFileName: originalFileName,
                fileType: extension,
                fileSize: fileSize,
                filePath: cleanPath,
                documentType: documentType,
                uploadedBy: uploadedBy);

            if (inserted)
                _logger.LogInformation("Document '{FileName}' saved for project {ProjectId}.", originalFileName, projectId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert Document '{FileName}' for project {ProjectId}.", originalFileName, projectId);

            await _auditLogger.LogAsync("Error", null,
                "Failed to insert Document entity via direct SQL.",
                ex.ToString(),
                metadata: new Dictionary<string, object?>(metadata)
                {
                    ["cleanPath"] = cleanPath,
                    ["stage"] = "map-document-failed"
                });

            await NotifyFailureAsync(uploadedBy, originalFileName,
                $"File '{originalFileName}' đã được quét sạch nhưng không thể lưu vào hệ thống.");
            return false;
        }
    }

    private async Task NotifyFailureAsync(Guid uploadedBy, string originalFileName, string reason)
    {
        try
        {
            await _notificationService.SendAsync(
                userId: uploadedBy,
                title: "Tải lên tài liệu thất bại",
                content: reason,
                type: NotificationType.Error,
                category: NotificationCategory.Project);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send failure notification to user {UserId}", uploadedBy);
        }
    }
}
