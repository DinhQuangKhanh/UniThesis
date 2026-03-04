using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UniThesis.API.Endpoints;
using UniThesis.Application.Features.Evaluations.DTOs;
using UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorHistory;

namespace UniThesis.API.Endpoints.Evaluations;

public class GetEvaluatorHistoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/evaluator/history", async (
                ISender sender,
                int page = 1,
                int pageSize = 10,
                string? search = null,
                string? result = null,
                string? dateRange = null,
                CancellationToken cancellationToken = default) =>
            {
                var query = new GetEvaluatorHistoryQuery(page, pageSize, search, result, dateRange);
                var dto = await sender.Send(query, cancellationToken);
                return Results.Ok(dto);
            })
            .RequireAuthorization()
            .WithTags("Evaluator")
            .WithName("GetEvaluatorHistory")
            .Produces<EvaluatorHistoryDto>()
            .Produces(401);
    }
}
