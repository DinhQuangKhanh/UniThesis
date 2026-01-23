using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record ProjectStartedEvent(Guid ProjectId, DateTime StartDate, DateTime Deadline) : DomainEventBase;
}
