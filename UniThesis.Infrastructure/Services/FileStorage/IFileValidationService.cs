namespace UniThesis.Infrastructure.Services.FileStorage
{
    public interface IFileValidationService
    {
        (bool IsValid, string? Error) ValidateFile(string fileName, long fileSize);
        (bool IsValid, string? Error) ValidateFile(string fileName, long fileSize, string[] allowedExtensions);
        string SanitizeFileName(string fileName);
        string GetContentType(string fileName);
    }
}
