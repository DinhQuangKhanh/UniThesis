using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.MeetingAggregate.Events
{
    public sealed record MeetingCompletedEvent(Guid MeetingId, Guid ProjectId) : DomainEventBase;
}
