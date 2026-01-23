using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.EvaluationAggregate.Events
{
    public sealed record SubmissionCreatedEvent(Guid SubmissionId, Guid ProjectId, Guid SubmittedBy, int SubmissionNumber) : DomainEventBase;
}
