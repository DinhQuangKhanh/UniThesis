using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Departments.Commands.SetDepartmentHead;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Admin;

public class SetDepartmentHeadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/admin/departments/{departmentId:int}/head", async (
                int departmentId,
                SetDepartmentHeadRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new SetDepartmentHeadCommand(departmentId, request.UserId);
                await sender.Send(command, cancellationToken);
                return NoContent("Thiết lập chở bộ phậm thành công.");
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Admin")
            .WithName("SetDepartmentHead")
            .Produces(204)
            .Produces(400)
            .Produces(401)
            .Produces(404);
    }
}

/// <summary>
/// Request body for setting department head.
/// </summary>
public record SetDepartmentHeadRequest(Guid UserId);
