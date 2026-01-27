namespace UniThesis.Infrastructure.Services.FileStorage
{
    public record FileUploadResult(bool Success, string? FilePath, string? PublicUrl, string? Error);
}
