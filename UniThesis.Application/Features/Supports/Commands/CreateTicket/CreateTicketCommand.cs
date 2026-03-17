using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Application.Features.Supports.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketCategory Category,
    TicketPriority Priority,
    Guid ReporterId) : ICommand<Guid>;
