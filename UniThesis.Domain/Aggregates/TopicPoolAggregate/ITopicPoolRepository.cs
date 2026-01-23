using UniThesis.Domain.Aggregates.TopicPoolAggregate.ValueObjects;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate
{
    public interface ITopicPoolRepository
    {
        Task<TopicPool?> GetByCodeAsync(TopicCode code, CancellationToken cancellationToken = default);
        Task<IEnumerable<TopicPool>> GetAvailableAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TopicPool>> GetByMajorIdAsync(int majorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TopicPool>> GetByProposedByAsync(Guid mentorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TopicPool>> GetExpiringAsync(int currentSemesterId, CancellationToken cancellationToken = default);
        Task<bool> ExistsCodeAsync(TopicCode code, CancellationToken cancellationToken = default);
        Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default);
    }
}
