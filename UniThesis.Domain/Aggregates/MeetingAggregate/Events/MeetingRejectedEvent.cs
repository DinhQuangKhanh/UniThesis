using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.MeetingAggregate.Events
{
    public sealed record MeetingRejectedEvent(Guid MeetingId, string? Reason) : DomainEventBase;
}
