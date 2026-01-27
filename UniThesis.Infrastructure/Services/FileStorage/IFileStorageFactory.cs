namespace UniThesis.Infrastructure.Services.FileStorage
{
    public interface IFileStorageFactory
    {
        IFileStorageService CreateStorageService();
    }
}
