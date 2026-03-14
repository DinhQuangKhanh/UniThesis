using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Events;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Infrastructure.Caching;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Evaluation
{
    public class EvaluationCompletedEventHandler : INotificationHandler<EvaluationCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEvaluationLogRepository _evaluationLogRepository;
        private readonly ICacheInvalidationService _cacheInvalidation;
        private readonly ILogger<EvaluationCompletedEventHandler> _logger;

        public EvaluationCompletedEventHandler(
            INotificationService notificationService,
            IEvaluationLogRepository evaluationLogRepository,
            ICacheInvalidationService cacheInvalidation,
            ILogger<EvaluationCompletedEventHandler> logger)
        {
            _notificationService = notificationService;
            _evaluationLogRepository = evaluationLogRepository;
            _cacheInvalidation = cacheInvalidation;
            _logger = logger;
        }

        public async Task Handle(EvaluationCompletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var evaluatorId = notification.EvaluatorId;

                await _evaluationLogRepository.AddAsync(new EvaluationLogDocument
                {
                    ProjectId = notification.ProjectId,
                    EvaluationSubmissionId = notification.SubmissionId,
                    Action = EvaluationAction.Completed,
                    Result = notification.Result,
                    PerformedBy = evaluatorId ?? Guid.Empty,
                    PerformedAt = DateTime.UtcNow
                }, cancellationToken);

                // Invalidate evaluator cache - dashboard, projects, history all change when evaluation completes
                if (evaluatorId.HasValue)
                {
                    await _cacheInvalidation.InvalidateEvaluatorCacheAsync(evaluatorId.Value, cancellationToken);
                }

                _logger.LogInformation("Evaluation completed: {ProjectId}, Result: {Result}", notification.ProjectId, notification.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {Event} for Project {ProjectId}",
                    nameof(EvaluationCompletedEvent), notification.ProjectId);
            }
        }
    }
}
