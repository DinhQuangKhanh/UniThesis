using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Events
{
    public sealed record RegistrationCancelledEvent(Guid TopicPoolId, Guid GroupId, string? Reason) : DomainEventBase;
}
