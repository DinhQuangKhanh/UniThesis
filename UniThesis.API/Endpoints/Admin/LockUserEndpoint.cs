using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Users.Commands.LockUser;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Admin;

public class LockUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/admin/users/{userId:guid}/lock", async (
                Guid userId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                await sender.Send(new LockUserCommand(userId), cancellationToken);
                return NoContent("Khóa thành công.");
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Admin")
            .WithName("LockUser")
            .Produces(204)
            .Produces(400)
            .Produces(401)
            .Produces(404);
    }
}
