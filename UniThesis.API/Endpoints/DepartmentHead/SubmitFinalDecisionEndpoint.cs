using MediatR;
using UniThesis.API.Extensions;
using UniThesis.API.Endpoints.DepartmentHead.Requests;
using UniThesis.Application.Features.Departments.Commands.SubmitFinalDecision;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.DepartmentHead;

public class SubmitFinalDecisionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/department-head/projects/{projectId:guid}/final-decision", async (
                Guid projectId,
                SubmitFinalDecisionRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new SubmitFinalDecisionCommand(projectId, request.Result, request.Notes);
                await sender.Send(command, cancellationToken);
                return NoContent("Quyết định cuối cùng đã được gửi thành công.");
            })
            .RequireAuthorization("RequireDepartmentHead")
            .WithTags("DepartmentHead")
            .WithName("SubmitFinalDecision")
            .Produces(204)
            .Produces(400)
            .Produces(401)
            .Produces(403);
    }
}
