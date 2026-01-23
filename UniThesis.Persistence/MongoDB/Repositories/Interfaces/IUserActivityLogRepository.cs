using UniThesis.Persistence.MongoDB.Documents;

namespace UniThesis.Persistence.MongoDB.Repositories.Interfaces
{
    public interface IUserActivityLogRepository
    {
        Task AddAsync(UserActivityLogDocument log, CancellationToken ct = default);
        Task<IEnumerable<UserActivityLogDocument>> GetByUserIdAsync(Guid userId, int limit = 100, CancellationToken ct = default);
    }
}
