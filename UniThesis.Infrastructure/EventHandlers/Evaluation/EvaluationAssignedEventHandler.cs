using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Events;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Evaluation
{
    public class EvaluationAssignedEventHandler : INotificationHandler<EvaluatorAssignedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEvaluationLogRepository _evaluationLogRepository;
        private readonly ILogger<EvaluationAssignedEventHandler> _logger;

        public EvaluationAssignedEventHandler(
            INotificationService notificationService,
            IEvaluationLogRepository evaluationLogRepository,
            ILogger<EvaluationAssignedEventHandler> logger)
        {
            _notificationService = notificationService;
            _evaluationLogRepository = evaluationLogRepository;
            _logger = logger;
        }

        public async Task Handle(EvaluatorAssignedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _evaluationLogRepository.AddAsync(new EvaluationLogDocument
                {
                    ProjectId = notification.ProjectId,
                    EvaluationSubmissionId = notification.SubmissionId,
                    Action = EvaluationAction.Assigned,
                    PerformedBy = notification.AssignedBy,
                    PerformedAt = DateTime.UtcNow,
                }, cancellationToken);

                _logger.LogInformation("Evaluator assigned: {EvaluatorId} to project {ProjectId}", notification.EvaluatorId, notification.ProjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {Event} for Project {ProjectId}",
                    nameof(EvaluatorAssignedEvent), notification.ProjectId);
            }
        }
    }
}
