using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate.ValueObjects;
using UniThesis.Domain.Enums.Project;
using UniThesis.Domain.Enums.TopicPool;
using UniThesis.Domain.Specifications.Projects;
using UniThesis.Domain.Specifications.TopicPools;
using UniThesis.Persistence.Common;

namespace UniThesis.Persistence.SqlServer.Repositories
{
    /// <summary>
    /// Repository implementation for Project aggregate using specifications.
    /// </summary>
    public class ProjectRepository : BaseRepository<Project, Guid>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context) { }

        public async Task<Project?> GetByCodeAsync(ProjectCode code, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
        }

        public async Task<Project?> GetWithMentorsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Mentors)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Project?> GetWithDocumentsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Project?> GetWithAllAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Mentors)
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetByMentorIdAsync(Guid mentorId, CancellationToken cancellationToken = default)
        {
            var spec = new ProjectByMentorSpec(mentorId);
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetBySemesterIdAsync(int semesterId, CancellationToken cancellationToken = default)
        {
            var spec = new ProjectBySemesterSpec(semesterId, includeDetails: true);
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
        {
            var spec = new ProjectByStatusSpec(status);
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetPendingEvaluationAsync(CancellationToken cancellationToken = default)
        {
            var spec = new ProjectPendingEvaluationSpec();
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<Project?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            var spec = new ProjectByGroupSpec(groupId);
            return await FirstOrDefaultAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetByMajorIdAsync(int majorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.MajorId == majorId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsCodeAsync(ProjectCode code, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(p => p.Code == code, cancellationToken);
        }

        public async Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default)
        {
            var prefix = $"PROJ-{year}-";
            var lastCode = await _dbSet
                .Where(p => EF.Functions.Like(p.Code.Value, $"{prefix}%"))
                .OrderByDescending(p => p.Code)
                .Select(p => p.Code.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastCode == null) return 1;

            var sequencePart = lastCode.Replace(prefix, "");
            return int.TryParse(sequencePart, out var seq) ? seq + 1 : 1;
        }

        public async Task<Dictionary<ProjectStatus, int>> GetStatusCountBySemesterAsync(int semesterId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.SemesterId == semesterId)
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
        }

        /// <summary>
        /// Gets projects that need modification using specification.
        /// </summary>
        public async Task<IEnumerable<Project>> GetNeedsModificationAsync(Guid? mentorId = null, CancellationToken cancellationToken = default)
        {
            var spec = new ProjectNeedsModificationSpec(mentorId);
            return await ListAsync(spec, cancellationToken);
        }

        /// <summary>
        /// Gets projects pending evaluation for a semester using specification.
        /// </summary>
        public async Task<IEnumerable<Project>> GetPendingEvaluationBySemesterAsync(int semesterId, CancellationToken cancellationToken = default)
        {
            var spec = new ProjectPendingEvaluationSpec(semesterId);
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<Dictionary<ProjectSourceType, int>> GetSourceTypeCountBySemesterAsync(int semesterId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.SemesterId == semesterId)
                .GroupBy(p => p.SourceType)
                .Select(g => new { Source = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Source, x => x.Count, cancellationToken);
        }

        public async Task<int> CountActivePoolTopicsByMentorAsync(Guid topicPoolId, Guid mentorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.TopicPoolId == topicPoolId &&
                           p.SourceType == ProjectSourceType.FromPool &&
                           (p.PoolStatus == PoolTopicStatus.Available || p.PoolStatus == PoolTopicStatus.Reserved) &&
                           p.Mentors.Any(m => m.MentorId == mentorId && m.IsActive))
                .CountAsync(cancellationToken);
        }

        public async Task<Dictionary<PoolTopicStatus, int>> GetPoolStatusCountsAsync(Guid topicPoolId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.TopicPoolId == topicPoolId && p.SourceType == ProjectSourceType.FromPool && p.PoolStatus.HasValue)
                .GroupBy(p => p.PoolStatus!.Value)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
        }

        public async Task<List<Guid>> GetPoolProjectIdsAsync(Guid topicPoolId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.TopicPoolId == topicPoolId && p.SourceType == ProjectSourceType.FromPool)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<int>> GetMentorTopicCountsInPoolAsync(Guid topicPoolId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.TopicPoolId == topicPoolId && p.SourceType == ProjectSourceType.FromPool)
                .SelectMany(p => p.Mentors.Where(m => m.IsActive))
                .GroupBy(m => m.MentorId)
                .Select(g => g.Count())
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Project>> GetExpirablePoolTopicsAsync(int currentSemesterId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.SourceType == ProjectSourceType.FromPool &&
                           p.PoolStatus == PoolTopicStatus.Available &&
                           p.ExpirationSemesterId.HasValue &&
                           p.ExpirationSemesterId.Value < currentSemesterId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Guid>> GetAvailableApprovedPoolTopicIdsAsync(Guid topicPoolId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.TopicPoolId == topicPoolId &&
                           p.SourceType == ProjectSourceType.FromPool &&
                           p.PoolStatus == PoolTopicStatus.Available &&
                           p.Status == ProjectStatus.Approved)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Project>> GetExpiringPoolTopicsWithMentorsAsync(int currentSemesterId, CancellationToken cancellationToken = default)
        {
            var spec = new ExpiringTopicsInPoolSpec(currentSemesterId);
            return await _dbSet
                .Where(spec.Criteria)
                .Include(p => p.Mentors)
                .ToListAsync(cancellationToken);
        }
    }
}
