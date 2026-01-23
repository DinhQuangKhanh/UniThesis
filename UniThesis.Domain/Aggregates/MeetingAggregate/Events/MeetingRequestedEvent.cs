using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.MeetingAggregate.Events
{
    public sealed record MeetingRequestedEvent(Guid MeetingId, Guid GroupId, Guid MentorId) : DomainEventBase;
}
