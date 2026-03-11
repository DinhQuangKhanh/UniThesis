using MediatR;
using UniThesis.Application.Features.Users.Queries.GetUsers;

namespace UniThesis.API.Endpoints.Admin;

public class GetUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/users", async (
                ISender sender,
                string? role,
                string? search,
                int page = 1,
                int pageSize = 20,
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(
                    new GetUsersQuery(role, search, page, pageSize), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Admin")
            .WithName("GetUsers")
            .Produces(200)
            .Produces(401);
    }
}
