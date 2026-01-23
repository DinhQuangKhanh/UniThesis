using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record LeaderChangedEvent(Guid GroupId, Guid? OldLeaderId, Guid NewLeaderId) : DomainEventBase;
}
