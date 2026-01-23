using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record ProjectModificationRequestedEvent(Guid ProjectId) : DomainEventBase;
}
