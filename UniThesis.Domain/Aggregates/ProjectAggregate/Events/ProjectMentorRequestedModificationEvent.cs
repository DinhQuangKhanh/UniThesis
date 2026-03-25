using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record ProjectMentorRequestedModificationEvent(Guid ProjectId, string? Feedback) : DomainEventBase;
}
