using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.TopicPools.DTOs;
using UniThesis.Application.Features.TopicPools.Queries.GetTopicPools;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.TopicPools;

public class GetTopicPoolsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topic-pools", async (
                ISender sender,
                int? majorId = null,
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(new GetTopicPoolsQuery(majorId), cancellationToken);
                return Ok(result);
            })
            .RequireAuthorization()
            .WithTags("TopicPools")
            .WithName("GetTopicPools")
            .Produces<List<TopicPoolDto>>()
            .Produces(401);
    }
}
