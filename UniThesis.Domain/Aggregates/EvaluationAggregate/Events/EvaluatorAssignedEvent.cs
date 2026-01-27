using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.EvaluationAggregate.Events
{
    public sealed record EvaluatorAssignedEvent(Guid SubmissionId, Guid EvaluatorId, Guid AssignedBy, Guid ProjectId) : DomainEventBase;
}
