namespace UniThesis.Application.Common.Interfaces;

using UniThesis.Application.Features.TopicPools.DTOs;

public interface ITopicPoolQueryService
{
    Task<List<TopicPoolDto>> GetTopicPoolsAsync(int? majorId, CancellationToken cancellationToken = default);
    Task<TopicPoolDto?> GetTopicPoolByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TopicPoolStatisticsDto> GetTopicPoolStatisticsAsync(Guid poolId, CancellationToken cancellationToken = default);
}
