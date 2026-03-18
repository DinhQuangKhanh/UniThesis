using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetTopicPoolStatistics;

public record GetTopicPoolStatisticsQuery(Guid PoolId) : ICachedQuery<TopicPoolStatisticsDto>
{
    public string? CacheKey => $"topic-pools:{PoolId}:stats";
}
