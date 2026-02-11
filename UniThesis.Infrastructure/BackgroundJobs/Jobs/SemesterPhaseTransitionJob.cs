using Microsoft.Extensions.Logging;
using UniThesis.Domain.Services;

namespace UniThesis.Infrastructure.BackgroundJobs.Jobs
{
    public class SemesterPhaseTransitionJob
    {
        private readonly ISemesterDomainService _semesterService;
        private readonly ILogger<SemesterPhaseTransitionJob> _logger;

        public SemesterPhaseTransitionJob(ISemesterDomainService semesterService, ILogger<SemesterPhaseTransitionJob> logger)
        {
            _semesterService = semesterService;
            _logger = logger;
        }

        public Task ExecuteAsync()
        {
            _logger.LogInformation("Starting SemesterPhaseTransitionJob");
            // TODO: Implementation: Check and transition semester phases
            _logger.LogInformation("SemesterPhaseTransitionJob completed");
            return Task.CompletedTask;
        }
    }
}
