using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Infrastructure.Services.Notification;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectSubmittedEventHandler : IDomainEventHandler<ProjectSubmittedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEvaluationLogRepository _evaluationLogRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ILogger<ProjectSubmittedEventHandler> _logger;

        public ProjectSubmittedEventHandler(
            INotificationService notificationService,
            IEvaluationLogRepository evaluationLogRepository,
            IUserActivityLogRepository activityLogRepository,
            ILogger<ProjectSubmittedEventHandler> logger)
        {
            _notificationService = notificationService;
            _evaluationLogRepository = evaluationLogRepository;
            _activityLogRepository = activityLogRepository;
            _logger = logger;
        }

        public async Task HandleAsync(ProjectSubmittedEvent @event, CancellationToken ct = default)
        {
            await _evaluationLogRepository.AddAsync(new EvaluationLogDocument
            {
                ProjectId = @event.ProjectId,
                Action = EvaluationAction.Submitted,
                PerformedBy = @event.SubmittedBy,
                PerformedAt = DateTime.UtcNow,
            }, ct);

            await _activityLogRepository.AddAsync(new UserActivityLogDocument
            {
                UserId = @event.SubmittedBy,
                Action = "ProjectSubmitted",
                EntityType = "Project",
                EntityId = @event.ProjectId,
                Timestamp = DateTime.UtcNow,
            }, ct);

            _logger.LogInformation("Project submitted for evaluation: {ProjectId}", @event.ProjectId);
        }
    }
}
