using UniThesis.Persistence.MongoDB.Documents;

namespace UniThesis.Persistence.MongoDB.Repositories.Interfaces
{
    public interface IUserActivityLogRepository
    {
        Task AddAsync(UserActivityLogDocument log, CancellationToken ct = default);
        Task<IEnumerable<UserActivityLogDocument>> GetByUserIdAsync(Guid userId, int limit = 100, CancellationToken ct = default);

        /// <summary>
        /// Returns a paginated, filterable list of activity logs.
        /// </summary>
        Task<(IEnumerable<UserActivityLogDocument> Items, long TotalCount)> GetPagedAsync(
            string? role = null,
            string? category = null,
            string? severity = null,
            string? searchTerm = null,
            DateTime? from = null,
            DateTime? to = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);
    }
}
