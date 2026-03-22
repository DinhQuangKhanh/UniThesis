using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record JoinRequestRejectedEvent(Guid GroupId, string GroupCode, Guid StudentId) : DomainEventBase;
}
