using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Infrastructure.Services.Notification;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Evaluation
{
    public class EvaluationAssignedEventHandler : IDomainEventHandler<EvaluatorAssignedEvent>
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

        public async Task HandleAsync(EvaluatorAssignedEvent @event, CancellationToken ct = default)
        {
            await _evaluationLogRepository.AddAsync(new EvaluationLogDocument
            {
                ProjectId = @event.ProjectId,
                EvaluationSubmissionId = @event.SubmissionId,
                Action = EvaluationAction.Assigned,
                PerformedBy = @event.AssignedBy,
                PerformedAt = DateTime.UtcNow,
            }, ct);

            _logger.LogInformation("Evaluator assigned: {EvaluatorId} to project {ProjectId}", @event.EvaluatorId, @event.ProjectId);
        }
    }
}
