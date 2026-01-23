using MongoDB.Driver;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Persistence.MongoDB.Repositories.Implementation
{
    public class UserActivityLogRepository : IUserActivityLogRepository
    {
        private readonly IMongoCollection<UserActivityLogDocument> _collection;

        public UserActivityLogRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<UserActivityLogDocument>(MongoDbContext.Collections.UserActivityLogs);
        }

        public async Task AddAsync(UserActivityLogDocument log, CancellationToken ct = default)
            => await _collection.InsertOneAsync(log, cancellationToken: ct);

        public async Task<IEnumerable<UserActivityLogDocument>> GetByUserIdAsync(Guid userId, int limit = 100, CancellationToken ct = default)
            => await _collection.Find(l => l.UserId == userId).SortByDescending(l => l.Timestamp).Limit(limit).ToListAsync(ct);
    }
}
