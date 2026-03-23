using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record JoinRequestedEvent(Guid GroupId, string GroupCode, Guid StudentId, Guid? LeaderId) : DomainEventBase;
}
