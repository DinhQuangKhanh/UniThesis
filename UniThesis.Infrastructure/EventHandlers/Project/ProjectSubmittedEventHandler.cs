using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Infrastructure.Services.Notification;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectSubmittedEventHandler : INotificationHandler<ProjectSubmittedEvent>
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

        public async Task Handle(ProjectSubmittedEvent notification, CancellationToken cancellationToken)
        {
            await _evaluationLogRepository.AddAsync(new EvaluationLogDocument
            {
                ProjectId = notification.ProjectId,
                Action = EvaluationAction.Submitted,
                PerformedBy = notification.SubmittedBy,
                PerformedAt = DateTime.UtcNow,
            }, cancellationToken);

            await _activityLogRepository.AddAsync(new UserActivityLogDocument
            {
                UserId = notification.SubmittedBy,
                Action = "ProjectSubmitted",
                EntityType = "Project",
                EntityId = notification.ProjectId,
                Timestamp = DateTime.UtcNow,
            }, cancellationToken);

            _logger.LogInformation("Project submitted for evaluation: {ProjectId}", notification.ProjectId);
        }
    }
}
