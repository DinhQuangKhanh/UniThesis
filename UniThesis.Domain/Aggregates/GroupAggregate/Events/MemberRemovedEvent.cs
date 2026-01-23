using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record MemberRemovedEvent(Guid GroupId, Guid StudentId) : DomainEventBase;
}
