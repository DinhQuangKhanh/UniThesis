using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Domain.Aggregates.SupportAggregate.Events
{
    public sealed record TicketCreatedEvent(Guid TicketId, string TicketCode, TicketCategory Category, TicketPriority Priority) : DomainEventBase;
}
