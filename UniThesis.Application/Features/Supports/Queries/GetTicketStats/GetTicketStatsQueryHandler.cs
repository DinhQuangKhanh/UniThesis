using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Supports.DTOs;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Application.Features.Supports.Queries.GetTicketStats;

public class GetTicketStatsQueryHandler : IQueryHandler<GetTicketStatsQuery, TicketStatsDto>
{
    private readonly ISupportTicketRepository _repository;

    public GetTicketStatsQueryHandler(ISupportTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<TicketStatsDto> Handle(GetTicketStatsQuery request, CancellationToken cancellationToken)
    {
        var counts = await _repository.GetStatusCountAsync(cancellationToken);
        
        var unread = counts.GetValueOrDefault(TicketStatus.Open, 0);
        var inProgress = counts.GetValueOrDefault(TicketStatus.InProgress, 0);
        var resolved = counts.GetValueOrDefault(TicketStatus.Resolved, 0);
        var closed = counts.GetValueOrDefault(TicketStatus.Closed, 0);

        return new TicketStatsDto
        {
            TotalTickets = unread + inProgress + resolved + closed,
            Unread = unread,
            InProgress = inProgress,
            Resolved = resolved + closed // Treat resolved and closed as resolved in stats
        };
    }
}
