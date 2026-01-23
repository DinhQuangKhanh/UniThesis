using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Events
{
    public sealed record DefenseScheduledEvent(Guid DefenseId, Guid GroupId, DateTime ScheduledDate) : DomainEventBase;
}
