using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UniThesis.Infrastructure.Services.FileStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly FileStorageSettings _settings;
        private readonly ILogger<LocalFileStorageService> _logger;
        private readonly string _basePath;

        public LocalFileStorageService(IOptions<FileStorageSettings> settings, ILogger<LocalFileStorageService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), _settings.BasePath);

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        public async Task<FileUploadResult> UploadAsync(Stream stream, string fileName, string folder, CancellationToken ct = default)
        {
            try
            {
                var folderPath = Path.Combine(_basePath, folder);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var filePath = Path.Combine(folderPath, uniqueFileName);
                var relativePath = Path.Combine(folder, uniqueFileName).Replace("\\", "/");

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await stream.CopyToAsync(fileStream, ct);

                _logger.LogInformation("File uploaded: {FilePath}", relativePath);

                return new FileUploadResult(true, relativePath, GetPublicUrl(relativePath), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
                return new FileUploadResult(false, null, null, ex.Message);
            }
        }

        public async Task<Stream?> DownloadAsync(string filePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            if (!File.Exists(fullPath)) return null;

            var memoryStream = new MemoryStream();
            await using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            await fileStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;
            return memoryStream;
        }

        public Task<bool> DeleteAsync(string filePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            if (!File.Exists(fullPath)) return Task.FromResult(false);

            File.Delete(fullPath);
            _logger.LogInformation("File deleted: {FilePath}", filePath);
            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(string filePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            return Task.FromResult(File.Exists(fullPath));
        }

        public string GetPublicUrl(string filePath)
        {
            return $"{_settings.BaseUrl}/{filePath}";
        }

        public Task<FileInfo?> GetFileInfoAsync(string filePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            if (!File.Exists(fullPath)) return Task.FromResult<FileInfo?>(null);

            var fileInfo = new System.IO.FileInfo(fullPath);
            return Task.FromResult<FileInfo?>(new FileInfo(
                fileInfo.Name,
                filePath,
                fileInfo.Length,
                GetContentType(fileInfo.Name),
                fileInfo.CreationTimeUtc
            ));
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
