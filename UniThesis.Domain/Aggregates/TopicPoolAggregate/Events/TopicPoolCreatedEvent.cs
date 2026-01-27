using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    /// <summary>
    /// Event raised when a new topic pool is created for a major.
    /// </summary>
    public sealed record TopicPoolCreatedEvent(
        Guid TopicPoolId,
        string Code,
        int MajorId
    ) : DomainEventBase;
}
