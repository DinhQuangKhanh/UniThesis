using Microsoft.Extensions.Options;

namespace UniThesis.Infrastructure.Services.FileStorage
{
    public class FileValidationService : IFileValidationService
    {
        private readonly FileStorageSettings _settings;

        public FileValidationService(IOptions<FileStorageSettings> settings)
        {
            _settings = settings.Value;
        }

        public (bool IsValid, string? Error) ValidateFile(string fileName, long fileSize)
        {
            return ValidateFile(fileName, fileSize, _settings.AllowedExtensions);
        }

        public (bool IsValid, string? Error) ValidateFile(string fileName, long fileSize, string[] allowedExtensions)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return (false, "File name is required.");

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return (false, $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");

            if (fileSize <= 0)
                return (false, "File is empty.");

            if (fileSize > _settings.MaxFileSizeBytes)
                return (false, $"File size exceeds maximum allowed size of {_settings.MaxFileSizeBytes / 1024 / 1024}MB.");

            return (true, null);
        }

        public string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
            return sanitized.Replace(" ", "_");
        }

        public string GetContentType(string fileName)
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
                ".gif" => "image/gif",
                ".zip" => "application/zip",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
