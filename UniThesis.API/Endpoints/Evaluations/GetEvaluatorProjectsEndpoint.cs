using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UniThesis.API.Endpoints;
using UniThesis.Application.Features.Evaluations.DTOs;
using UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorProjects;

namespace UniThesis.API.Endpoints.Evaluations;

public class GetEvaluatorProjectsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/evaluator/projects", async (
                ISender sender,
                int page = 1,
                int pageSize = 10,
                string? search = null,
                int? semesterId = null,
                int? majorId = null,
                string? result = null,
                CancellationToken cancellationToken = default) =>
            {
                var query = new GetEvaluatorProjectsQuery(page, pageSize, search, semesterId, majorId, result);
                var dto = await sender.Send(query, cancellationToken);
                return Results.Ok(dto);
            })
            .RequireAuthorization()
            .WithTags("Evaluator")
            .WithName("GetEvaluatorProjects")
            .Produces<EvaluatorProjectsDto>()
            .Produces(401);
    }
}
