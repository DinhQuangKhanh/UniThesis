using MongoDB.Driver;
using UniThesis.Persistence.MongoDB.Documents;

namespace UniThesis.Persistence.MongoDB.Indexes
{
    /// <summary>
    /// Configures MongoDB indexes for optimal query performance.
    /// </summary>
    public static class MongoIndexConfiguration
    {
        /// <summary>
        /// Creates all required indexes for the collections.
        /// </summary>
        public static async Task CreateIndexesAsync(MongoDbContext context)
        {
            await CreateEvaluationLogIndexesAsync(context);
            await CreateProjectModificationIndexesAsync(context);
            await CreateNotificationIndexesAsync(context);
            await CreateConversationIndexesAsync(context);
            await CreateMessageIndexesAsync(context);
            await CreateUserActivityLogIndexesAsync(context);
            await CreateSystemAuditLogIndexesAsync(context);
        }

        private static async Task CreateEvaluationLogIndexesAsync(MongoDbContext context)
        {
            var collection = context.GetCollection<EvaluationLogDocument>(MongoDbContext.Collections.EvaluationLogs);
            var indexKeys = Builders<EvaluationLogDocument>.IndexKeys;

            await collection.Indexes.CreateManyAsync(new[]
            {
            new CreateIndexModel<EvaluationLogDocument>(indexKeys.Ascending(x => x.ProjectId)),
            new CreateIndexModel<EvaluationLogDocument>(indexKeys.Ascending(x => x.EvaluationSubmissionId)),
            new CreateIndexModel<EvaluationLogDocument>(indexKeys.Ascending(x => x.PerformedBy)),
            new CreateIndexModel<EvaluationLogDocument>(indexKeys.Descending(x => x.PerformedAt)),
            new CreateIndexModel<EvaluationLogDocument>(indexKeys.Combine(
                indexKeys.Ascending(x => x.ProjectId),
                indexKeys.Descending(x => x.PerformedAt)))
        });
        }

        private static async Task CreateProjectModificationIndexesAsync(MongoDbContext context)
        {
            var collection = context.GetCollection<ProjectModificationHistoryDocument>(MongoDbContext.Collections.ProjectModifications);
            var indexKeys = Builders<ProjectModificationHistoryDocument>.IndexKeys;

            await collection.Indexes.CreateManyAsync(new[]
            {
            new CreateIndexModel<ProjectModificationHistoryDocument>(indexKeys.Ascending(x => x.ProjectId)),
            new CreateIndexModel<ProjectModificationHistoryDocument>(indexKeys.Descending(x => x.ModifiedAt)),
            new CreateIndexModel<ProjectModificationHistoryDocument>(indexKeys.Combine(
                indexKeys.Ascending(x => x.ProjectId),
                indexKeys.Descending(x => x.ModifiedAt)))
        });
        }

        private static async Task CreateNotificationIndexesAsync(MongoDbContext context)
        {
            var collection = context.GetCollection<NotificationDocument>(MongoDbContext.Collections.Notifications);
            var indexKeys = Builders<NotificationDocument>.IndexKeys;

            await collection.Indexes.CreateManyAsync(new[]
            {
            new CreateIndexModel<NotificationDocument>(indexKeys.Ascending(x => x.UserId)),
            new CreateIndexModel<NotificationDocument>(indexKeys.Combine(
                indexKeys.Ascending(x => x.UserId),
                indexKeys.Ascending(x => x.IsRead))),
            new CreateIndexModel<NotificationDocument>(indexKeys.Combine(
                indexKeys.Ascending(x => x.UserId),
                indexKeys.Descending(x => x.CreatedAt))),
            new CreateIndexModel<NotificationDocument>(
                indexKeys.Descending(x => x.CreatedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(90) }) // TTL index - auto delete after 90 days
        });
        }

        private static async Task CreateConversationIndexesAsync(MongoDbContext context)
        {
            var collection = context.GetCollection<ConversationDocument>(MongoDbContext.Collections.Conversations);
            var indexKeys = Builders<ConversationDocument>.IndexKeys;

            await collection.Indexes.CreateManyAsync(new[]
            {
            new CreateIndexModel<ConversationDocument>(indexKeys.Ascending(x => x.ParticipantIds)),
            new CreateIndexModel<ConversationDocument>(indexKeys.Ascending(x => x.GroupId)),
            new CreateIndexModel<ConversationDocument>(indexKeys.Descending(x => x.LastMessageAt))
        });
        }

        private static async Task CreateMessageIndexesAsync(MongoDbContext context)
        {
            var collection = context.GetCollection<MessageDocument>(MongoDbContext.Collections.Messages);
            var indexKeys = Builders<MessageDocument>.IndexKeys;

            await collection.Indexes.CreateManyAsync(new[]
            {
            new CreateIndexModel<MessageDocument>(indexKeys.Ascending(x => x.ConversationId)),
            new CreateIndexModel<MessageDocument>(indexKeys.Combine(
                indexKeys.Ascending(x => x.ConversationId),
                indexKeys.Descending(x => x.CreatedAt))),
            new CreateIndexModel<MessageDocument>(indexKeys.Ascending(x => x.SenderId))
        });
        }

        private static async Task CreateUserActivityLogIndexesAsync(MongoDbContext context)
        {
            var collection = context.GetCollection<UserActivityLogDocument>(MongoDbContext.Collections.UserActivityLogs);
            var indexKeys = Builders<UserActivityLogDocument>.IndexKeys;

            await collection.Indexes.CreateManyAsync(new[]
            {
            new CreateIndexModel<UserActivityLogDocument>(indexKeys.Ascending(x => x.UserId)),
            new CreateIndexModel<UserActivityLogDocument>(indexKeys.Descending(x => x.Timestamp)),
            new CreateIndexModel<UserActivityLogDocument>(
                indexKeys.Descending(x => x.Timestamp),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(365) }) // TTL - auto delete after 1 year
        });
        }

        private static async Task CreateSystemAuditLogIndexesAsync(MongoDbContext context)
        {
            var collection = context.GetCollection<SystemAuditLogDocument>(MongoDbContext.Collections.SystemAuditLogs);
            var indexKeys = Builders<SystemAuditLogDocument>.IndexKeys;

            await collection.Indexes.CreateManyAsync(new[]
            {
            new CreateIndexModel<SystemAuditLogDocument>(indexKeys.Combine(
                indexKeys.Ascending(x => x.EntityType),
                indexKeys.Ascending(x => x.EntityId))),
            new CreateIndexModel<SystemAuditLogDocument>(indexKeys.Descending(x => x.Timestamp)),
            new CreateIndexModel<SystemAuditLogDocument>(indexKeys.Ascending(x => x.PerformedBy))
        });
        }
    }
}
