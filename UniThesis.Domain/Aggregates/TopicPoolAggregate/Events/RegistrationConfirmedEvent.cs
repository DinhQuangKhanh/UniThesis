using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    public sealed record RegistrationConfirmedEvent(Guid TopicPoolId, Guid GroupId) : DomainEventBase;
}
