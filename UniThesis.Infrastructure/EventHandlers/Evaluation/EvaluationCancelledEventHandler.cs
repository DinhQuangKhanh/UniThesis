using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Infrastructure.Services.Notification;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Evaluation
{
    /// <summary>
    /// Handler for EvaluationCancelledEvent.
    /// </summary>
    public class EvaluationCancelledEventHandler : IDomainEventHandler<EvaluationCancelledEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEvaluationLogRepository _evaluationLogRepository;
        private readonly ILogger<EvaluationCancelledEventHandler> _logger;

        public EvaluationCancelledEventHandler(
            INotificationService notificationService,
            IEvaluationLogRepository evaluationLogRepository,
            ILogger<EvaluationCancelledEventHandler> logger)
        {
            _notificationService = notificationService;
            _evaluationLogRepository = evaluationLogRepository;
            _logger = logger;
        }

        public async Task HandleAsync(EvaluationCancelledEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Evaluation cancelled: SubmissionId={SubmissionId}, ProjectId={ProjectId}",
                @event.SubmissionId, @event.ProjectId);

            try
            {
                // Log the cancellation
                var log = new EvaluationLogDocument
                {
                    ProjectId = @event.ProjectId,
                    Action = EvaluationAction.Cancelled,
                    PerformedAt = DateTime.UtcNow,
                };

                await _evaluationLogRepository.AddAsync(log, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling EvaluationCancelledEvent for submission {SubmissionId}",
                    @event.SubmissionId);
            }
        }
    }
}
