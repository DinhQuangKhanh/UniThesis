using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetPoolTopics;

public class GetPoolTopicsQueryHandler : IQueryHandler<GetPoolTopicsQuery, GetPoolTopicsResult>
{
    private readonly ITopicPoolQueryService _queryService;

    public GetPoolTopicsQueryHandler(ITopicPoolQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<GetPoolTopicsResult> Handle(GetPoolTopicsQuery request, CancellationToken cancellationToken)
    {
        return await _queryService.GetPoolTopicsAsync(
            request.MajorId,
            request.Search,
            request.PoolStatus,
            request.SortBy,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}
