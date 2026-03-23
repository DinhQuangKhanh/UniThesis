using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record JoinRequestApprovedEvent(Guid GroupId, string GroupCode, Guid StudentId) : DomainEventBase;
}
