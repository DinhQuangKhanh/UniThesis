using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record ProjectGroupAssignedEvent(Guid ProjectId, Guid GroupId) : DomainEventBase;
}
