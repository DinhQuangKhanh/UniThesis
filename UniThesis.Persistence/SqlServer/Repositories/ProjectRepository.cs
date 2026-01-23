using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate.ValueObjects;
using UniThesis.Domain.Enums.Mentor;
using UniThesis.Domain.Enums.Project;
using UniThesis.Persistence.Common;

namespace UniThesis.Persistence.SqlServer.Repositories
{
    public class ProjectRepository : BaseRepository<Project, Guid>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context) { }

        public async Task<Project?> GetByCodeAsync(ProjectCode code, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(p => p.Code == code, ct);

        public async Task<Project?> GetWithMentorsAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.Include(p => p.Mentors).FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<Project?> GetWithDocumentsAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.Include(p => p.Documents).FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<Project?> GetWithAllAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.Include(p => p.Mentors).Include(p => p.Documents).FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IEnumerable<Project>> GetByMentorIdAsync(Guid mentorId, CancellationToken ct = default)
            => await _dbSet.Include(p => p.Mentors)
                .Where(p => p.Mentors.Any(m => m.MentorId == mentorId && m.Status == ProjectMentorStatus.Active))
                .OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

        public async Task<IEnumerable<Project>> GetBySemesterIdAsync(int semesterId, CancellationToken ct = default)
            => await _dbSet.Where(p => p.SemesterId == semesterId).OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

        public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken ct = default)
            => await _dbSet.Where(p => p.Status == status).OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

        public async Task<IEnumerable<Project>> GetPendingEvaluationAsync(CancellationToken ct = default)
            => await _dbSet.Include(p => p.Mentors).Where(p => p.Status == ProjectStatus.PendingEvaluation).OrderBy(p => p.SubmittedAt).ToListAsync(ct);

        public async Task<Project?> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(p => p.GroupId == groupId, ct);

        public async Task<IEnumerable<Project>> GetByMajorIdAsync(int majorId, CancellationToken ct = default)
            => await _dbSet.Where(p => p.MajorId == majorId).OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

        public async Task<bool> ExistsCodeAsync(ProjectCode code, CancellationToken ct = default)
            => await _dbSet.AnyAsync(p => p.Code == code, ct);

        public async Task<int> GetNextSequenceAsync(int year, CancellationToken ct = default)
        {
            var prefix = $"PROJ-{year}-";
            var lastCode = await _dbSet.Where(p => EF.Functions.Like(p.Code.Value, $"{prefix}%"))
                .OrderByDescending(p => p.Code).Select(p => p.Code.Value).FirstOrDefaultAsync(ct);
            if (lastCode == null) return 1;
            var sequencePart = lastCode.Replace(prefix, "");
            return int.TryParse(sequencePart, out var seq) ? seq + 1 : 1;
        }

        public async Task<Dictionary<ProjectStatus, int>> GetStatusCountBySemesterAsync(int semesterId, CancellationToken ct = default)
            => await _dbSet.Where(p => p.SemesterId == semesterId).GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Status, x => x.Count, ct);
    }
}
