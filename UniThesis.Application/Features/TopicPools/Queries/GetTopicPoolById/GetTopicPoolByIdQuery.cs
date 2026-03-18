using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetTopicPoolById;

public record GetTopicPoolByIdQuery(Guid Id) : ICachedQuery<TopicPoolDto>
{
    public string? CacheKey => $"topic-pools:{Id}";
}
