using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Supports.DTOs;

namespace UniThesis.Application.Features.Supports.Queries.GetTicketById;

public record GetTicketByIdQuery(Guid Id) : IQuery<TicketDto>;
