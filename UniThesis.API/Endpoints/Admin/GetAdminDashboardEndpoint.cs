using MediatR;
using UniThesis.Application.Common;
using UniThesis.Application.Features.Dashboard.DTOs;
using UniThesis.Application.Features.Dashboard.Queries.GetAdminDashboard;

namespace UniThesis.API.Endpoints.Admin;

public class GetAdminDashboardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/dashboard", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetAdminDashboardQuery(), cancellationToken);
                return Results.Ok(ApiResponse.Ok(result));
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Admin")
            .WithName("GetAdminDashboard")
            .Produces<ApiResponse<AdminDashboardDto>>()
            .Produces(401);
    }
}
