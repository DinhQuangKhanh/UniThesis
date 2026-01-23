using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.ValueObjects;
using UniThesis.Domain.Enums.TopicPool;
using UniThesis.Domain.Specifications.TopicPools;
using UniThesis.Persistence.Common;

namespace UniThesis.Persistence.SqlServer.Repositories
{
    /// <summary>
    /// Repository implementation for TopicPool aggregate using specifications.
    /// </summary>
    public class TopicPoolRepository : BaseRepository<TopicPool, Guid>, ITopicPoolRepository
    {
        public TopicPoolRepository(AppDbContext context) : base(context) { }

        public async Task<TopicPool?> GetByCodeAsync(TopicCode code, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Registrations)
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
        }

        public async Task<IEnumerable<TopicPool>> GetAvailableAsync(CancellationToken cancellationToken = default)
        {
            var spec = new AvailableTopicsSpec();
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<TopicPool>> GetByMajorIdAsync(int majorId, CancellationToken cancellationToken = default)
        {
            var spec = new TopicsByMajorSpec(majorId, availableOnly: true);
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<TopicPool>> GetByProposedByAsync(Guid mentorId, CancellationToken cancellationToken = default)
        {
            var spec = new TopicsByMentorSpec(mentorId);
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<TopicPool>> GetExpiringAsync(int currentSemesterId, CancellationToken cancellationToken = default)
        {
            var spec = new ExpiringTopicsSpec(currentSemesterId);
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<bool> ExistsCodeAsync(TopicCode code, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(t => t.Code == code, cancellationToken);
        }

        public async Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default)
        {
            var prefix = $"TP-{year}-";
            var lastCode = await _dbSet
                .Where(t => EF.Functions.Like(t.Code.Value, $"{prefix}%"))
                .OrderByDescending(t => t.Code)
                .Select(t => t.Code.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastCode == null) return 1;

            var sequencePart = lastCode.Replace(prefix, "");
            return int.TryParse(sequencePart, out var seq) ? seq + 1 : 1;
        }

        /// <summary>
        /// Gets topics by mentor with specific status using specification.
        /// </summary>
        public async Task<IEnumerable<TopicPool>> GetByMentorAndStatusAsync(Guid mentorId, TopicPoolStatus status, CancellationToken cancellationToken = default)
        {
            var spec = new TopicsByMentorSpec(mentorId, status);
            return await ListAsync(spec, cancellationToken);
        }

        /// <summary>
        /// Gets all topics by major including non-available using specification.
        /// </summary>
        public async Task<IEnumerable<TopicPool>> GetAllByMajorIdAsync(int majorId, CancellationToken cancellationToken = default)
        {
            var spec = new TopicsByMajorSpec(majorId, availableOnly: false);
            return await ListAsync(spec, cancellationToken);
        }

        /// <summary>
        /// Gets topic with registrations.
        /// </summary>
        public async Task<TopicPool?> GetWithRegistrationsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Registrations)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        /// <summary>
        /// Gets available topics for a specific major using specification.
        /// </summary>
        public async Task<IEnumerable<TopicPool>> GetAvailableByMajorAsync(int majorId, CancellationToken cancellationToken = default)
        {
            var spec = new AvailableTopicsSpec(majorId);
            return await ListAsync(spec, cancellationToken);
        }
    }
}
