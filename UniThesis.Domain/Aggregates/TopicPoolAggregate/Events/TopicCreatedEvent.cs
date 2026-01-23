using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    public sealed record TopicCreatedEvent(Guid TopicPoolId, Guid ProposedBy) : DomainEventBase;
}
