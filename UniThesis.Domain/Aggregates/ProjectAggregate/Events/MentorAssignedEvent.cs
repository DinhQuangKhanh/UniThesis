using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Mentor;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record MentorAssignedEvent(Guid ProjectId, Guid MentorId, MentorRole Role) : DomainEventBase;
}
