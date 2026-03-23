using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Supports.Commands.ReplyTicket;

[ActionLog("Reply Ticket", "Support")]
public record ReplyTicketCommand(
    Guid TicketId,
    Guid SenderId,
    string Content) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["tickets:"];
}
