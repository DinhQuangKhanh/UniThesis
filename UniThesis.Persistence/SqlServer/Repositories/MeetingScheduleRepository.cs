using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.MeetingAggregate;
using UniThesis.Domain.Enums.Meeting;
using UniThesis.Persistence.Common;

namespace UniThesis.Persistence.SqlServer.Repositories
{
    /// <summary>
    /// Repository implementation for MeetingSchedule aggregate.
    /// </summary>
    public class MeetingScheduleRepository : BaseRepository<MeetingSchedule, Guid>, IMeetingScheduleRepository
    {
        public MeetingScheduleRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<MeetingSchedule>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.GroupId == groupId)
                .OrderByDescending(m => m.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MeetingSchedule>> GetByMentorIdAsync(Guid mentorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.MentorId == mentorId)
                .OrderByDescending(m => m.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MeetingSchedule>> GetPendingByMentorIdAsync(Guid mentorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.MentorId == mentorId && m.Status == MeetingStatus.Pending)
                .OrderBy(m => m.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MeetingSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.ScheduledDate >= startDate && m.ScheduledDate <= endDate)
                .OrderBy(m => m.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MeetingSchedule>> GetUpcomingByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.GroupId == groupId &&
                           m.ScheduledDate >= DateTime.UtcNow &&
                           (m.Status == MeetingStatus.Pending || m.Status == MeetingStatus.Approved))
                .OrderBy(m => m.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets approved meetings for a mentor.
        /// </summary>
        public async Task<IEnumerable<MeetingSchedule>> GetApprovedByMentorIdAsync(Guid mentorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.MentorId == mentorId && m.Status == MeetingStatus.Approved)
                .OrderBy(m => m.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets meeting statistics by status.
        /// </summary>
        public async Task<Dictionary<MeetingStatus, int>> GetStatusCountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .GroupBy(m => m.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
        }

        /// <summary>
        /// Gets upcoming meetings within specified days.
        /// </summary>
        public async Task<IEnumerable<MeetingSchedule>> GetUpcomingAsync(int days = 7, CancellationToken cancellationToken = default)
        {
            var endDate = DateTime.UtcNow.AddDays(days);
            return await _dbSet
                .Where(m => m.Status == MeetingStatus.Approved &&
                           m.ScheduledDate >= DateTime.UtcNow &&
                           m.ScheduledDate <= endDate)
                .OrderBy(m => m.ScheduledDate)
                .ToListAsync(cancellationToken);
        }
    }
}
