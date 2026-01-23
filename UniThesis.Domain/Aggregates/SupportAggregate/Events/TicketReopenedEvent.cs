using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.SupportAggregate.Events
{
    public sealed record TicketReopenedEvent(Guid TicketId) : DomainEventBase;
}
