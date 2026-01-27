using Microsoft.Extensions.Options;

namespace UniThesis.Infrastructure.Services.FileStorage
{
    public class FileStorageFactory : IFileStorageFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FileStorageSettings _settings;

        public FileStorageFactory(IServiceProvider serviceProvider, IOptions<FileStorageSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _settings = settings.Value;
        }

        public IFileStorageService CreateStorageService()
        {
            return _settings.Provider.ToLower() switch
            {
                "azureblob" or "azure" => (IFileStorageService)_serviceProvider.GetService(typeof(AzureBlobStorageService))!,
                _ => (IFileStorageService)_serviceProvider.GetService(typeof(LocalFileStorageService))!
            };
        }
    }
}
