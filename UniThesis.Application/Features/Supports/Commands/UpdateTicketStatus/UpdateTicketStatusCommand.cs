using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Application.Features.Supports.Commands.UpdateTicketStatus;

public record UpdateTicketStatusCommand(
    Guid TicketId,
    TicketStatus Status) : ICommand;
