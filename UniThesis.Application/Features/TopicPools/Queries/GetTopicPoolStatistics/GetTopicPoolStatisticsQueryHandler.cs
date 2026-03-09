using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetTopicPoolStatistics;

public class GetTopicPoolStatisticsQueryHandler : IQueryHandler<GetTopicPoolStatisticsQuery, TopicPoolStatisticsDto>
{
    private readonly ITopicPoolQueryService _queryService;

    public GetTopicPoolStatisticsQueryHandler(ITopicPoolQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<TopicPoolStatisticsDto> Handle(GetTopicPoolStatisticsQuery request, CancellationToken cancellationToken)
    {
        return await _queryService.GetTopicPoolStatisticsAsync(request.PoolId, cancellationToken);
    }
}
