using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniThesis.Persistence.MongoDB.Documents
{
    /// <summary>
    /// Document for user activity logs.
    /// </summary>
    public class UserActivityLogDocument
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? EntityType { get; set; }

        public Guid? EntityId { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [BsonExtraElements]
        public BsonDocument? Details { get; set; }
    }
}
