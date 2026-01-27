using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    /// <summary>
    /// Event raised when a group requests to register for a topic from the pool.
    /// </summary>
    public sealed record TopicRegistrationRequestedEvent(
        Guid RegistrationId,
        Guid ProjectId,
        Guid GroupId,
        Guid RegisteredBy
    ) : DomainEventBase;
}
