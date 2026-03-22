using MediatR;
using UniThesis.API.Extensions;
using UniThesis.API.Endpoints.DepartmentHead.Requests;
using UniThesis.Application.Features.Departments.Commands.AssignEvaluator;
using static UniThesis.API.Extensions.ApiResponseExtensions;

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
                return NoContent("Gán thẩm pồi thành công.");
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
