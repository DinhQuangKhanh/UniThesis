using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.EvaluationAggregate.Events
{
    public sealed record EvaluationCancelledEvent(Guid SubmissionId, Guid ProjectId) : DomainEventBase;
}
