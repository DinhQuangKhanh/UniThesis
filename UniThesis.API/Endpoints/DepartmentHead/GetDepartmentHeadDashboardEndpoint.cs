using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Common;
using UniThesis.Application.Features.Dashboard.DTOs;
using UniThesis.Application.Features.Dashboard.Queries.GetDepartmentHeadDashboard;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.DepartmentHead;

public class GetDepartmentHeadDashboardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/department-head/dashboard", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetDepartmentHeadDashboardQuery(), cancellationToken);
                return Ok(result);
            })
            .RequireAuthorization()
            .WithTags("DepartmentHead")
            .WithName("GetDepartmentHeadDashboard")
            .Produces<ApiResponse<DepartmentHeadDashboardDto>>()
            .Produces(401);
    }
}
