using MediatR;
using UniThesis.Application.Features.TopicPools.DTOs;
using UniThesis.Application.Features.TopicPools.Queries.GetTopicPoolStatistics;

namespace UniThesis.API.Endpoints.TopicPools;

public class GetTopicPoolStatisticsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topic-pools/{id:guid}/statistics", async (
                ISender sender,
                Guid id,
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(new GetTopicPoolStatisticsQuery(id), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithTags("TopicPools")
            .WithName("GetTopicPoolStatistics")
            .Produces<TopicPoolStatisticsDto>()
            .Produces(401)
            .Produces(404);
    }
}
