using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetTopicPools;

public record GetTopicPoolsQuery(int? MajorId = null) : ICachedQuery<List<TopicPoolDto>>
{
    public string? CacheKey => $"topic-pools:list:{MajorId}";
}
