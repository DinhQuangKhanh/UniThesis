using MediatR;
using UniThesis.Application.Features.Departments.Commands.AssignEvaluator;

namespace UniThesis.API.Endpoints.DepartmentHead;

public class AssignEvaluatorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/department-head/assign-evaluator", async (
                AssignEvaluatorRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new AssignEvaluatorCommand(
                    request.ProjectId,
                    request.EvaluatorId,
                    request.EvaluatorOrder);

                await sender.Send(command, cancellationToken);
                return Results.NoContent();
            })
            .RequireAuthorization("RequireDepartmentHead")
            .WithTags("DepartmentHead")
            .WithName("AssignEvaluator")
            .Produces(204)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404);
    }
}

/// <summary>
/// Request body for assigning an evaluator to a project.
/// </summary>
public record AssignEvaluatorRequest(Guid ProjectId, Guid EvaluatorId, int EvaluatorOrder);
