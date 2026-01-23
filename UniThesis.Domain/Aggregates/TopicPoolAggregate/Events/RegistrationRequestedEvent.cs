using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    public sealed record RegistrationRequestedEvent(Guid TopicPoolId, Guid GroupId, Guid RegisteredBy) : DomainEventBase;
}
