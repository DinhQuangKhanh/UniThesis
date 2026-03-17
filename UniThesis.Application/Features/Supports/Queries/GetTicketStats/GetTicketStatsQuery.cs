using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Supports.DTOs;

namespace UniThesis.Application.Features.Supports.Queries.GetTicketStats;

public record GetTicketStatsQuery : IQuery<TicketStatsDto>;
