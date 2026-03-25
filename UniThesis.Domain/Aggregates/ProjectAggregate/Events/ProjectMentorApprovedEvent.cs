using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record ProjectMentorApprovedEvent(Guid ProjectId, Guid MentorId) : DomainEventBase;
}
