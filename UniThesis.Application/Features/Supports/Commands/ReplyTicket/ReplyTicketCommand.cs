using MediatR;
using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Supports.Commands.ReplyTicket;

public record ReplyTicketCommand(
    Guid TicketId,
    Guid SenderId,
    string Content) : ICommand;
