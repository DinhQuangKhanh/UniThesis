using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Application.Features.Supports.Commands.CreateTicket;

[ActionLog("Create Ticket", "Support")]
public record CreateTicketCommand(
    string Title,
    string Description,
    TicketCategory Category,
    TicketPriority Priority,
    Guid ReporterId) : ICacheInvalidatingCommand<Guid>
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["tickets:"];
}
