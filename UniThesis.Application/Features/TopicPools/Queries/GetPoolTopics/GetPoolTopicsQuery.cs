using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetPoolTopics;

/// <summary>
/// Query to get a paginated list of pool topics with optional filters.
/// Implements ICachedQuery with L1/L2 caching — search queries bypass cache.
/// </summary>
public record GetPoolTopicsQuery(
    int? MajorId,
    string? Search,
    int? PoolStatus,
    string? SortBy,
    int Page = 1,
    int PageSize = 12
) : ICachedQuery<GetPoolTopicsResult>
{
    /// <summary>
    /// Cache key based on filter combination. Returns null when searching to avoid cache key explosion.
    /// </summary>
    public string? CacheKey => string.IsNullOrWhiteSpace(Search)
        ? $"pool-topics:major:{MajorId ?? 0}:status:{PoolStatus ?? -1}:sort:{SortBy ?? "newest"}:p:{Page}:s:{PageSize}"
        : null;

    public TimeSpan? L1Expiration => TimeSpan.FromMinutes(2);
    public TimeSpan? L2Expiration => TimeSpan.FromMinutes(10);
}
