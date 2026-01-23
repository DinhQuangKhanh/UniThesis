using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    public sealed record TopicSelectedEvent(Guid TopicPoolId, Guid GroupId) : DomainEventBase;
}
