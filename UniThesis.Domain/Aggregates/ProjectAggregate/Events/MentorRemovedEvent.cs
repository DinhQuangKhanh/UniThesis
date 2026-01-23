using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record MentorRemovedEvent(Guid ProjectId, Guid MentorId) : DomainEventBase;
}
