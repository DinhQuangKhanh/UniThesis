using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.SupportAggregate.Events
{
    public sealed record TicketAssignedEvent(Guid TicketId, Guid AssigneeId) : DomainEventBase;
}
