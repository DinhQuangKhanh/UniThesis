using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Infrastructure.Caching;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectRejectedEventHandler : INotificationHandler<ProjectRejectedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IProjectEvaluatorAssignmentRepository _assignmentRepository;
        private readonly ICacheInvalidationService _cacheInvalidation;
        private readonly ILogger<ProjectRejectedEventHandler> _logger;

        public ProjectRejectedEventHandler(
            INotificationService notificationService,
            IProjectEvaluatorAssignmentRepository assignmentRepository,
            ICacheInvalidationService cacheInvalidation,
            ILogger<ProjectRejectedEventHandler> logger)
        {
            _notificationService = notificationService;
            _assignmentRepository = assignmentRepository;
            _cacheInvalidation = cacheInvalidation;
            _logger = logger;
        }

        public async Task Handle(ProjectRejectedEvent notification, CancellationToken cancellationToken)
        {
            // Invalidate cache for all evaluators assigned to this project
            var assignments = await _assignmentRepository.GetActiveByProjectIdAsync(notification.ProjectId, cancellationToken);
            foreach (var assignment in assignments)
            {
                await _cacheInvalidation.InvalidateEvaluatorCacheAsync(assignment.EvaluatorId, cancellationToken);
            }

            _logger.LogInformation("Project rejected: {ProjectId}", notification.ProjectId);
        }
    }
}
