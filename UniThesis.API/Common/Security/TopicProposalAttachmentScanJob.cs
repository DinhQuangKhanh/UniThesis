using Microsoft.AspNetCore.Http;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Document;
using UniThesis.Infrastructure.Services.FileStorage;

namespace UniThesis.API.Common.Security;

internal sealed class TopicProposalAttachmentScanJob
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITopicProposalMalwareScanner _malwareScanner;
    private readonly IMalwareScanAuditLogger _auditLogger;
    private readonly ILogger<TopicProposalAttachmentScanJob> _logger;

    public TopicProposalAttachmentScanJob(
        IFileStorageService fileStorageService,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ITopicProposalMalwareScanner malwareScanner,
        IMalwareScanAuditLogger auditLogger,
        ILogger<TopicProposalAttachmentScanJob> logger)
    {
        _fileStorageService = fileStorageService;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _malwareScanner = malwareScanner;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid projectId, Guid poolId, Guid uploadedBy, string quarantinePath, string originalFileName)
    {
        var metadata = new Dictionary<string, object?>
        {
            ["projectId"] = projectId,
            ["poolId"] = poolId,
            ["uploadedBy"] = uploadedBy,
            ["quarantinePath"] = quarantinePath,
            ["stage"] = "background-scan"
        };

        try
        {
            await using var downloaded = await _fileStorageService.DownloadAsync(quarantinePath);
            if (downloaded is null)
            {
                await _auditLogger.LogAsync(
                    verdict: "Error",
                    file: null,
                    message: "Quarantine file not found when executing background malware scan.",
                    scannerResponse: null,
                    metadata: metadata);
                return;
            }

            if (!downloaded.CanSeek)
            {
                var mem = new MemoryStream();
                await downloaded.CopyToAsync(mem);
                mem.Position = 0;
                downloaded.Dispose();
                await ProcessStreamAsync(projectId, poolId, uploadedBy, quarantinePath, originalFileName, mem, metadata);
                return;
            }

            downloaded.Position = 0;
            await ProcessStreamAsync(projectId, poolId, uploadedBy, quarantinePath, originalFileName, downloaded, metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Background malware scan job failed for quarantine path {QuarantinePath}", quarantinePath);
            await _auditLogger.LogAsync(
                verdict: "Error",
                file: null,
                message: "Background malware scan job failed.",
                scannerResponse: ex.Message,
                metadata: metadata);
        }
    }

    private async Task ProcessStreamAsync(
        Guid projectId,
        Guid poolId,
        Guid uploadedBy,
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

        var scanResult = await _malwareScanner.ScanAsync([formFile]);
        if (!scanResult.IsClean)
        {
            await _auditLogger.LogAsync(
                verdict: "Quarantined",
                file: formFile,
                message: $"Attachment kept in quarantine after scan result: {scanResult.Message}",
                scannerResponse: null,
                metadata: metadata);
            return;
        }

        var cleanFolder = $"topic-proposals/clean/{poolId:N}/{DateTime.UtcNow:yyyyMMdd}";
        var moveResult = await _fileStorageService.MoveAsync(quarantinePath, cleanFolder);
        if (!moveResult.Success || string.IsNullOrWhiteSpace(moveResult.FilePath))
        {
            await _auditLogger.LogAsync(
                verdict: "Error",
                file: formFile,
                message: "Failed to promote clean attachment from quarantine.",
                scannerResponse: moveResult.Error,
                metadata: metadata);
            return;
        }

        var project = await _projectRepository.GetWithDocumentsAsync(projectId);
        if (project is null)
        {
            await _auditLogger.LogAsync(
                verdict: "Error",
                file: formFile,
                message: "Không tìm thấy đề tài để gắn file đính kèm sau khi scan sạch.",
                scannerResponse: null,
                metadata: new Dictionary<string, object?>(metadata)
                {
                    ["cleanPath"] = moveResult.FilePath,
                    ["stage"] = "map-document"
                });
            return;
        }

        var alreadyMapped = project.Documents.Any(d =>
            string.Equals(d.FilePath, moveResult.FilePath, StringComparison.OrdinalIgnoreCase));

        if (!alreadyMapped)
        {
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            project.AddDocument(
                fileName: Path.GetFileName(moveResult.FilePath),
                originalFileName: originalFileName,
                fileType: extension,
                fileSize: formFile.Length,
                filePath: moveResult.FilePath,
                documentType: DocumentType.Proposal,
                uploadedBy: uploadedBy);

            await _unitOfWork.SaveChangesAsync();
        }

        await _auditLogger.LogAsync(
            verdict: "Promoted",
            file: formFile,
            message: "Attachment scanned clean and promoted out of quarantine.",
            scannerResponse: null,
            metadata: new Dictionary<string, object?>(metadata)
            {
                ["cleanPath"] = moveResult.FilePath,
                ["documentMapped"] = !alreadyMapped,
                ["quarantineDeleted"] = true,
                ["stage"] = "promote"
            });
    }
}
