using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.StudentGroups.DTOs;
using UniThesis.Application.Features.StudentGroups.Queries.GetMentorGroups;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.StudentGroups;

public class GetMentorGroupsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/student-groups/mentor", async (
                [FromQuery] int? semesterId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetMentorGroupsQuery(semesterId), cancellationToken);
                return Ok(result);
            })
            .RequireAuthorization()
            .WithTags("StudentGroups")
            .WithName("GetMentorGroups")
            .Produces<List<MentorGroupDto>>()
            .Produces(401);
    }
}
