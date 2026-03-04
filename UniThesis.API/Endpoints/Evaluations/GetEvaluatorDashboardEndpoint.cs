using MediatR;
using UniThesis.Application.Features.Evaluations.DTOs;
using UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorDashboard;

namespace UniThesis.API.Endpoints.Evaluations;

public class GetEvaluatorDashboardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/evaluator/dashboard", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetEvaluatorDashboardQuery(), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithTags("Evaluator")
            .WithName("GetEvaluatorDashboard")
            .Produces<EvaluatorDashboardDto>()
            .Produces(401);
    }
}
