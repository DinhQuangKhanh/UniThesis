using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record ProjectSubmittedToMentorEvent(Guid ProjectId, Guid SubmittedBy) : DomainEventBase;
}
