using Microsoft.Extensions.Logging;

namespace UniThesis.Infrastructure.BackgroundJobs.Jobs
{
    public class DataCleanupJob
    {
        private readonly ILogger<DataCleanupJob> _logger;

        public DataCleanupJob(ILogger<DataCleanupJob> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Starting DataCleanupJob");
            // Implementation: Clean up old logs, temporary files, etc.
            await Task.CompletedTask;
            _logger.LogInformation("DataCleanupJob completed");
        }
    }
}
