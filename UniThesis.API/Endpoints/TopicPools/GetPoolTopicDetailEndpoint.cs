using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.TopicPools.DTOs;
using UniThesis.Application.Features.TopicPools.Queries.GetPoolTopicDetail;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.TopicPools;

public class GetPoolTopicDetailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topic-pools/topics/{projectId:guid}", async (
                ISender sender,
                Guid projectId,
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(
                    new GetPoolTopicDetailQuery(projectId),
                    cancellationToken);

                return result is not null
                    ? Ok(result)
                    : Results.NotFound();
            })
            .RequireAuthorization()
            .WithTags("TopicPools")
            .WithName("GetPoolTopicDetail")
            .Produces<PoolTopicDetailDto>()
            .Produces(404)
            .Produces(401);
    }
}
