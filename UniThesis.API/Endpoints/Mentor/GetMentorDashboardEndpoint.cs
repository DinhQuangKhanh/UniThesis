using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Common;
using UniThesis.Application.Features.Dashboard.DTOs;
using UniThesis.Application.Features.Dashboard.Queries.GetMentorDashboard;
using UniThesis.Infrastructure.Authorization.Policies;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Mentor;

public class GetMentorDashboardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/mentor/dashboard", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetMentorDashboardQuery(), cancellationToken);
                return Ok(result);
            })
            .RequireAuthorization(PolicyNames.RequireMentor)
            .WithTags("Mentor")
            .WithName("GetMentorDashboard")
            .Produces<ApiResponse<MentorDashboardDto>>()
            .Produces(401);
    }
}
