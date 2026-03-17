using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Common.Primitives;

namespace UniThesis.Domain.Aggregates.SupportAggregate.Events
{
    /// <summary>
    /// Event raised when a new message/reply is added to a support ticket.
    /// </summary>
    public sealed record TicketMessageAddedEvent(
        Guid TicketId,
        Guid MessageId,
        Guid SenderId) : DomainEventBase;
}

