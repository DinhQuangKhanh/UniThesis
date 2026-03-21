using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Application.Features.Supports.Commands.UpdateTicketStatus;

[ActionLog("Update Ticket Status", "Support")]
public record UpdateTicketStatusCommand(
    Guid TicketId,
    TicketStatus Status) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["tickets:"];
}
