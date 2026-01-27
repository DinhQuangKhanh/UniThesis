using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    public sealed record TopicPoolSuspendedEvent(Guid TopicPoolId) : DomainEventBase;
}
