using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate;

namespace UniThesis.Domain.Services
{
    public interface ITopicPoolDomainService
    {
        /// <summary>
        /// Generates a unique topic code.
        /// </summary>
        Task<string> GenerateTopicCodeAsync(int year, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calculates the expiration semester for a topic.
        /// </summary>
        Task<int> CalculateExpirationSemesterAsync(int createdSemesterId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Expires all topics that have passed their expiration date.
        /// </summary>
        Task<int> ExpireTopicsAsync(int currentSemesterId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Converts a topic from the pool to a project.
        /// </summary>
        Task<Project> ConvertToProjectAsync(TopicPool topic, Guid groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets topics expiring in the next semester.
        /// </summary>
        Task<IEnumerable<TopicPool>> GetExpiringTopicsAsync(int currentSemesterId, CancellationToken cancellationToken = default);
    }
}
