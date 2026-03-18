using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetPoolTopicDetail;

/// <summary>
/// Query to get full details of a single pool topic by project ID.
/// Cached per project ID with longer TTL since topic details rarely change.
/// </summary>
public record GetPoolTopicDetailQuery(Guid ProjectId) : ICachedQuery<PoolTopicDetailDto?>
{
    public string? CacheKey => $"pool-topics:detail:{ProjectId}";
    public TimeSpan? L1Expiration => TimeSpan.FromMinutes(5);
    public TimeSpan? L2Expiration => TimeSpan.FromMinutes(15);
}
