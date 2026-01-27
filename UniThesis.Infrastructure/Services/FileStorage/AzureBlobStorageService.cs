using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UniThesis.Infrastructure.Services.FileStorage
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly FileStorageSettings _settings;
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public AzureBlobStorageService(IOptions<FileStorageSettings> settings, ILogger<AzureBlobStorageService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            if (string.IsNullOrEmpty(_settings.AzureConnectionString))
                throw new InvalidOperationException("Azure connection string is not configured.");

            var blobServiceClient = new BlobServiceClient(_settings.AzureConnectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(_settings.AzureContainerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<FileUploadResult> UploadAsync(Stream stream, string fileName, string folder, CancellationToken ct = default)
        {
            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var blobPath = string.IsNullOrEmpty(folder) ? uniqueFileName : $"{folder}/{uniqueFileName}";

                var blobClient = _containerClient.GetBlobClient(blobPath);

                var contentType = GetContentType(fileName);
                var headers = new BlobHttpHeaders { ContentType = contentType };

                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers }, ct);

                _logger.LogInformation("File uploaded to Azure: {BlobPath}", blobPath);

                return new FileUploadResult(true, blobPath, blobClient.Uri.ToString(), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file to Azure: {FileName}", fileName);
                return new FileUploadResult(false, null, null, ex.Message);
            }
        }

        public async Task<Stream?> DownloadAsync(string filePath, CancellationToken ct = default)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            if (!await blobClient.ExistsAsync(ct)) return null;

            var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream, ct);
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<bool> DeleteAsync(string filePath, CancellationToken ct = default)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: ct);

            if (response.Value)
                _logger.LogInformation("File deleted from Azure: {FilePath}", filePath);

            return response.Value;
        }

        public async Task<bool> ExistsAsync(string filePath, CancellationToken ct = default)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            return await blobClient.ExistsAsync(ct);
        }

        public string GetPublicUrl(string filePath)
        {
            return _containerClient.GetBlobClient(filePath).Uri.ToString();
        }

        public async Task<FileInfo?> GetFileInfoAsync(string filePath, CancellationToken ct = default)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            if (!await blobClient.ExistsAsync(ct)) return null;

            var properties = await blobClient.GetPropertiesAsync(cancellationToken: ct);
            return new FileInfo(
                Path.GetFileName(filePath),
                filePath,
                properties.Value.ContentLength,
                properties.Value.ContentType,
                properties.Value.CreatedOn.UtcDateTime
            );
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }
    }
}
