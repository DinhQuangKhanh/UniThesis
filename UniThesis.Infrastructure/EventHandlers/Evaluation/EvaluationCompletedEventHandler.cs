using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Infrastructure.Services.Notification;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Evaluation
{
    public class EvaluationCompletedEventHandler : IDomainEventHandler<EvaluationCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEvaluationLogRepository _evaluationLogRepository;
        private readonly ILogger<EvaluationCompletedEventHandler> _logger;

        public EvaluationCompletedEventHandler(
            INotificationService notificationService,
            IEvaluationLogRepository evaluationLogRepository,
            ILogger<EvaluationCompletedEventHandler> logger)
        {
            _notificationService = notificationService;
            _evaluationLogRepository = evaluationLogRepository;
            _logger = logger;
        }

        public async Task HandleAsync(EvaluationCompletedEvent @event, CancellationToken ct = default)
        {
            await _evaluationLogRepository.AddAsync(new EvaluationLogDocument
            {
                ProjectId = @event.ProjectId,
                EvaluationSubmissionId = @event.SubmissionId,
                Action = EvaluationAction.Completed,
                Result = @event.Result,
                PerformedBy = (Guid)@event.EvaluatorId,
                PerformedAt = DateTime.UtcNow
            }, ct);

            _logger.LogInformation("Evaluation completed: {ProjectId}, Result: {Result}", @event.ProjectId, @event.Result);
        }
    }
}
