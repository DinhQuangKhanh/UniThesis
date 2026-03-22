using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Topics.DTOs;
using UniThesis.Application.Features.Topics.Queries.GetTopicsInPool;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Topics;

public class GetTopicsInPoolEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topics", async (
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
                    new GetTopicsInPoolQuery(majorId, search, poolStatus, sortBy, page, pageSize),
                    cancellationToken);
                return Ok(result);
            })
            .RequireAuthorization()
            .WithTags("Topics")
            .WithName("GetTopicsInPool")
            .Produces<GetTopicsInPoolResult>()
            .Produces(401);
    }
}
