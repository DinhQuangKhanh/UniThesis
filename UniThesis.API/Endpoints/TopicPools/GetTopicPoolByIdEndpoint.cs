using MediatR;
using UniThesis.Application.Features.TopicPools.DTOs;
using UniThesis.Application.Features.TopicPools.Queries.GetTopicPoolById;

namespace UniThesis.API.Endpoints.TopicPools;

public class GetTopicPoolByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topic-pools/{id:guid}", async (
                ISender sender,
                Guid id,
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(new GetTopicPoolByIdQuery(id), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithTags("TopicPools")
            .WithName("GetTopicPoolById")
            .Produces<TopicPoolDto>()
            .Produces(401)
            .Produces(404);
    }
}
