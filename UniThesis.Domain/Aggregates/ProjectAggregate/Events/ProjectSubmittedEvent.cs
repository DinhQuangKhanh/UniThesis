using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record ProjectSubmittedEvent(Guid ProjectId, Guid SubmittedBy, int SubmissionNumber) : DomainEventBase;
}
