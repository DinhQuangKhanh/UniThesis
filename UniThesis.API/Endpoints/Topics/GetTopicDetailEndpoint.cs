using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Topics.DTOs;
using UniThesis.Application.Features.Topics.Queries.GetTopicDetail;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Topics;

public class GetTopicDetailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topics/{topicId:guid}", async (
                ISender sender,
                Guid topicId,
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(
                    new GetTopicDetailQuery(topicId),
                    cancellationToken);

                return result is not null
                    ? Ok(result)
                    : Results.NotFound();
            })
            .RequireAuthorization()
            .WithTags("Topics")
            .WithName("GetTopicDetail")
            .Produces<TopicDetailDto>()
            .Produces(404)
            .Produces(401);
    }
}
