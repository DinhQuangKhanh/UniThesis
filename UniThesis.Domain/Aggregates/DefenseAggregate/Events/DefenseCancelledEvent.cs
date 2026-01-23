using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Events
{
    public sealed record DefenseCancelledEvent(Guid DefenseId) : DomainEventBase;
}
