using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Infrastructure.BackgroundJobs.Jobs
{
    public class SemesterPhaseTransitionJob
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SemesterPhaseTransitionJob> _logger;

        public SemesterPhaseTransitionJob(
            ISemesterRepository semesterRepository,
            IUnitOfWork unitOfWork,
            ILogger<SemesterPhaseTransitionJob> logger)
        {
            _semesterRepository = semesterRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting SemesterPhaseTransitionJob - checking for upcoming phases.");

            // Find all semesters where ANY phase starts in exactly 3 days
            var upcomingSemesters = await _semesterRepository.GetSemestersWithPhaseStartingInAsync(3, cancellationToken);
            var count = 0;
            var targetDate = DateTime.UtcNow.Date.AddDays(3);

            foreach (var semester in upcomingSemesters)
            {
                // Check for Registration and Evaluation phases that start in 3 days
                var upcomingPhases = semester.Phases.Where(p =>
                    (p.Type == SemesterPhaseType.Registration || p.Type == SemesterPhaseType.Evaluation)
                    && p.StartDate.Date == targetDate);

                foreach (var phase in upcomingPhases)
                {
                    _logger.LogInformation("Found upcoming {PhaseType} phase {PhaseId} in Semester {SemesterId}",
                        phase.Type, phase.Id, semester.Id);

                    // Trigger the Domain Event
                    semester.NotifyUpcomingPhase(phase.Id);
                    count++;
                }
            }

            if (count > 0)
            {
                // Save changes will trigger the DomainEventInterceptor, which will dispatch PhaseUpcomingEvent to MediatR
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Dispatched {Count} PhaseUpcomingEvents for upcoming phases.", count);
            }

            _logger.LogInformation("SemesterPhaseTransitionJob completed");
        }
    }
}
