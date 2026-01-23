using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.DefenseAggregate;
using UniThesis.Domain.Enums.Defense;
using UniThesis.Persistence.Common;

namespace UniThesis.Persistence.SqlServer.Repositories
{
    /// <summary>
    /// Repository implementation for DefenseSchedule aggregate.
    /// </summary>
    public class DefenseScheduleRepository : BaseRepository<DefenseSchedule, Guid>, IDefenseScheduleRepository
    {
        public DefenseScheduleRepository(AppDbContext context) : base(context) { }

        public override async Task<DefenseSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Council)
                    .ThenInclude(c => c!.Members)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<DefenseSchedule?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Council)
                .FirstOrDefaultAsync(d => d.GroupId == groupId, cancellationToken);
        }

        public async Task<IEnumerable<DefenseSchedule>> GetBySemesterIdAsync(int semesterId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Council)
                .Where(d => d.Council != null && d.Council.SemesterId == semesterId)
                .OrderBy(d => d.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DefenseSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Council)
                .Where(d => d.ScheduledDate >= startDate && d.ScheduledDate <= endDate)
                .OrderBy(d => d.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DefenseSchedule>> GetByCouncilIdAsync(int councilId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Council)
                .Where(d => d.CouncilId == councilId)
                .OrderBy(d => d.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets scheduled defenses.
        /// </summary>
        public async Task<IEnumerable<DefenseSchedule>> GetScheduledAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Council)
                .Where(d => d.Status == DefenseScheduleStatus.Scheduled)
                .OrderBy(d => d.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets upcoming defenses within specified days.
        /// </summary>
        public async Task<IEnumerable<DefenseSchedule>> GetUpcomingAsync(int days = 7, CancellationToken cancellationToken = default)
        {
            var endDate = DateTime.UtcNow.AddDays(days);
            return await _dbSet
                .Include(d => d.Council)
                .Where(d => d.Status == DefenseScheduleStatus.Scheduled &&
                           d.ScheduledDate >= DateTime.UtcNow &&
                           d.ScheduledDate <= endDate)
                .OrderBy(d => d.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets defense statistics by status.
        /// </summary>
        public async Task<Dictionary<DefenseScheduleStatus, int>> GetStatusCountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .GroupBy(d => d.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
        }
    }
}
