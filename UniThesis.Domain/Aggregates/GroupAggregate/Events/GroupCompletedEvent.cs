using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate.Events
{
    public sealed record GroupCompletedEvent(Guid GroupId) : DomainEventBase;
}
