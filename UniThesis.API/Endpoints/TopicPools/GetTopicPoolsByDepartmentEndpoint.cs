using MediatR;
using UniThesis.Application.Features.TopicPools.DTOs;
using UniThesis.Application.Features.TopicPools.Queries.GetTopicPoolsByDepartment;

namespace UniThesis.API.Endpoints.TopicPools;

public class GetTopicPoolsByDepartmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topic-pools/by-department", async (
                ISender sender,
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(new GetTopicPoolsByDepartmentQuery(), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithTags("TopicPools")
            .WithName("GetTopicPoolsByDepartment")
            .Produces<List<DepartmentWithPoolsDto>>()
            .Produces(401);
    }
}
