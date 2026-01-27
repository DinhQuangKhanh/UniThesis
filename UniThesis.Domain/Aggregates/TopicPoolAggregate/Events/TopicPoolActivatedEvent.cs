using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    public sealed record TopicPoolActivatedEvent(Guid TopicPoolId) : DomainEventBase;
}
