using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Enums.Ticket;

using UniThesis.Application.Common.Models;

namespace UniThesis.Application.Features.Supports.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketCategory Category,
    TicketPriority Priority,
    Guid ReporterId,
    IEnumerable<FileAttachmentDto>? Attachments = null) : ICommand<Guid>;
