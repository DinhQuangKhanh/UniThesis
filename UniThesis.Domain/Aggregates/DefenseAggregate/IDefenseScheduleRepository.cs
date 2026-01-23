using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.DefenseAggregate
{
    public interface IDefenseScheduleRepository : IRepository<DefenseSchedule, Guid>
    {
        Task<DefenseSchedule?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DefenseSchedule>> GetBySemesterIdAsync(int semesterId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DefenseSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<DefenseSchedule>> GetByCouncilIdAsync(int councilId, CancellationToken cancellationToken = default);
    }
}
