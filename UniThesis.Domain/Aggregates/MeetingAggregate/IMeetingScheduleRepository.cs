using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.MeetingAggregate
{
    public interface IMeetingScheduleRepository : IRepository<MeetingSchedule, Guid>
    {
        Task<IEnumerable<MeetingSchedule>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MeetingSchedule>> GetByMentorIdAsync(Guid mentorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MeetingSchedule>> GetPendingByMentorIdAsync(Guid mentorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MeetingSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<MeetingSchedule>> GetUpcomingByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    }
}
