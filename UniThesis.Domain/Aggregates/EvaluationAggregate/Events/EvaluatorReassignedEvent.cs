using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.EvaluationAggregate.Events
{
    public sealed record EvaluatorReassignedEvent(Guid SubmissionId, Guid? PreviousEvaluatorId, Guid NewEvaluatorId, Guid ReassignedBy, Guid projectId) : DomainEventBase;
}
