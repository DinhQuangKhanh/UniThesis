using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Events;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Infrastructure.Caching;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Evaluation
{
    /// <summary>
    /// Handler for EvaluationCancelledEvent.
    /// </summary>
    public class EvaluationCancelledEventHandler : INotificationHandler<EvaluationCancelledEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEvaluationLogRepository _evaluationLogRepository;
        private readonly IProjectEvaluatorAssignmentRepository _assignmentRepository;
        private readonly ICacheInvalidationService _cacheInvalidation;
        private readonly ILogger<EvaluationCancelledEventHandler> _logger;

        public EvaluationCancelledEventHandler(
            INotificationService notificationService,
            IEvaluationLogRepository evaluationLogRepository,
            IProjectEvaluatorAssignmentRepository assignmentRepository,
            ICacheInvalidationService cacheInvalidation,
            ILogger<EvaluationCancelledEventHandler> logger)
        {
            _notificationService = notificationService;
            _evaluationLogRepository = evaluationLogRepository;
            _assignmentRepository = assignmentRepository;
            _cacheInvalidation = cacheInvalidation;
            _logger = logger;
        }

        public async Task Handle(EvaluationCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Evaluation cancelled: SubmissionId={SubmissionId}, ProjectId={ProjectId}",
                notification.SubmissionId, notification.ProjectId);

            try
            {
                // Log the cancellation
                var log = new EvaluationLogDocument
                {
                    ProjectId = notification.ProjectId,
                    Action = EvaluationAction.Cancelled,
                    PerformedAt = DateTime.UtcNow,
                };

                await _evaluationLogRepository.AddAsync(log, cancellationToken);

                // Invalidate cache for all evaluators assigned to this project
                var assignments = await _assignmentRepository.GetActiveByProjectIdAsync(notification.ProjectId, cancellationToken);
                foreach (var assignment in assignments)
                {
                    await _cacheInvalidation.InvalidateEvaluatorCacheAsync(assignment.EvaluatorId, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling EvaluationCancelledEvent for submission {SubmissionId}",
                    notification.SubmissionId);
            }
        }
    }
}
