using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record InvitationRejectedEvent(Guid GroupId, string GroupCode, Guid InviterId, Guid InviteeId) : DomainEventBase;
}
