using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Domain.Aggregates.SupportAggregate.Events
{
    public sealed record TicketStatusChangedEvent(Guid TicketId, TicketStatus OldStatus, TicketStatus NewStatus) : DomainEventBase;
}
