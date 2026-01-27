namespace UniThesis.Infrastructure.Services.FileStorage
{
    public class FileStorageSettings
    {
        public const string SectionName = "FileStorageSettings";
        public string Provider { get; set; } = "Local"; // Local, AzureBlob
        public string BasePath { get; set; } = "uploads";
        public string BaseUrl { get; set; } = "/files";
        public long MaxFileSizeBytes { get; set; } = 50 * 1024 * 1024; // 50MB
        public string[] AllowedExtensions { get; set; } = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".zip" };

        // Azure Blob Settings
        public string? AzureConnectionString { get; set; }
        public string? AzureContainerName { get; set; } = "unithesis-files";
    }
}
