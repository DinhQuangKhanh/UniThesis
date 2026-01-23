using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Events
{
    public sealed record DefenseCompletedEvent(Guid DefenseId, Guid GroupId) : DomainEventBase;
}
