using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Supports.DTOs;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Application.Features.Supports.Queries.GetTickets;

public record GetTicketsQuery(
    Guid ReporterId,
    bool IsAdmin,
    string? SearchTerm = null,
    TicketStatus? Status = null,
    TicketPriority? Priority = null) : IQuery<List<TicketListDto>>;
