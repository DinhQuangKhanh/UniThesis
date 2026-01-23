using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Group;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record MemberAddedEvent(Guid GroupId, Guid StudentId, GroupMemberRole Role) : DomainEventBase;
}
