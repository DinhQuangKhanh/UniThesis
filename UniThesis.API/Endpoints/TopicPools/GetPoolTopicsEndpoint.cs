using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.TopicPools.DTOs;
using UniThesis.Application.Features.TopicPools.Queries.GetPoolTopics;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.TopicPools;

public class GetPoolTopicsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topic-pools/topics", async (
                ISender sender,
                int? majorId = null,
                string? search = null,
                int? poolStatus = null,
                string? sortBy = null,
                int page = 1,
                int pageSize = 12,
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(
                    new GetPoolTopicsQuery(majorId, search, poolStatus, sortBy, page, pageSize),
                    cancellationToken);
                return Ok(result);
            })
            .RequireAuthorization()
            .WithTags("TopicPools")
            .WithName("GetPoolTopics")
            .Produces<GetPoolTopicsResult>()
            .Produces(401);
    }
}
