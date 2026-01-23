using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record ProjectCreatedEvent(Guid ProjectId, string ProjectCode, ProjectSourceType SourceType) : DomainEventBase;
}
