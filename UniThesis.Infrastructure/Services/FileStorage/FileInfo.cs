namespace UniThesis.Infrastructure.Services.FileStorage
{
    public record FileInfo(string FileName, string FilePath, long FileSize, string ContentType, DateTime UploadedAt);
}
