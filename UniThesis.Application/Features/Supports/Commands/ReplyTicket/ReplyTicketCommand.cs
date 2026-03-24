using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

using UniThesis.Application.Common.Models;

namespace UniThesis.Application.Features.Supports.Commands.ReplyTicket;

[ActionLog("Reply Ticket", "Support")]
public record ReplyTicketCommand(
    Guid TicketId,
    Guid SenderId,
    string Content,
    IEnumerable<FileAttachmentDto>? Attachments = null) : ICommand;
