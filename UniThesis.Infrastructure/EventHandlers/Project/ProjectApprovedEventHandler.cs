using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Infrastructure.Caching;
using UniThesis.Application.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectApprovedEventHandler : INotificationHandler<ProjectApprovedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IProjectEvaluatorAssignmentRepository _assignmentRepository;
        private readonly ICacheInvalidationService _cacheInvalidation;
        private readonly ILogger<ProjectApprovedEventHandler> _logger;

        public ProjectApprovedEventHandler(
            INotificationService notificationService,
            IProjectEvaluatorAssignmentRepository assignmentRepository,
            ICacheInvalidationService cacheInvalidation,
            ILogger<ProjectApprovedEventHandler> logger)
        {
            _notificationService = notificationService;
            _assignmentRepository = assignmentRepository;
            _cacheInvalidation = cacheInvalidation;
            _logger = logger;
        }

        public async Task Handle(ProjectApprovedEvent notification, CancellationToken cancellationToken)
        {
            // Invalidate cache for all evaluators assigned to this project
            var assignments = await _assignmentRepository.GetActiveByProjectIdAsync(notification.ProjectId, cancellationToken);
            foreach (var assignment in assignments)
            {
                await _cacheInvalidation.InvalidateEvaluatorCacheAsync(assignment.EvaluatorId, cancellationToken);
            }

            _logger.LogInformation("Project approved: {ProjectId}", notification.ProjectId);
        }
    }
}
