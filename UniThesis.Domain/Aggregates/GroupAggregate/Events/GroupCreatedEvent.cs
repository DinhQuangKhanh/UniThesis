using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record GroupCreatedEvent(Guid GroupId, string GroupCode, int SemesterId) : DomainEventBase;
}
