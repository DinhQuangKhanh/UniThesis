using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Events
{
    public sealed record DefensePostponedEvent(Guid DefenseId, DateTime NewDate) : DomainEventBase;
}
