using System.Text.RegularExpressions;
using MongoDB.Bson;
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

        public async Task<(IEnumerable<UserActivityLogDocument> Items, long TotalCount)> GetPagedAsync(
            string? role = null,
            string? category = null,
            string? severity = null,
            string? searchTerm = null,
            DateTime? from = null,
            DateTime? to = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            var builder = Builders<UserActivityLogDocument>.Filter;
            var filters = new List<FilterDefinition<UserActivityLogDocument>>();

            if (!string.IsNullOrWhiteSpace(role))
            {
                // Case-insensitive exact match so "evaluator" matches "Evaluator" stored in DB
                var roleRegex = new BsonRegularExpression(
                    $"^{Regex.Escape(role)}$", "i");
                filters.Add(builder.Regex(x => x.UserRole, roleRegex));
            }

            if (!string.IsNullOrWhiteSpace(category))
                filters.Add(builder.Eq(x => x.Category, category));

            if (!string.IsNullOrWhiteSpace(severity))
                filters.Add(builder.Eq(x => x.Severity, severity));

            if (from.HasValue)
                filters.Add(builder.Gte(x => x.Timestamp, from.Value));

            if (to.HasValue)
                filters.Add(builder.Lte(x => x.Timestamp, to.Value));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var regex = new BsonRegularExpression(searchTerm, "i");
                filters.Add(builder.Or(
                    builder.Regex(x => x.UserName, regex),
                    builder.Regex(x => x.Action, regex),
                    builder.Regex(x => x.EntityType, regex),
                    builder.Regex(x => x.UserEmail, regex)));
            }

            var filter = filters.Count > 0 ? builder.And(filters) : builder.Empty;

            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

            var items = await _collection
                .Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}
