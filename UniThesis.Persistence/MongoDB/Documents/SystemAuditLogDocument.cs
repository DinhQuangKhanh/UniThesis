using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniThesis.Persistence.MongoDB.Documents
{
    public class SystemAuditLogDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public Guid? EntityId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? PerformedBy { get; set; }

        public string? PerformedByName { get; set; }
        public string? OldValuesJson { get; set; }
        public string? NewValuesJson { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [BsonExtraElements]
        public BsonDocument? Metadata { get; set; }
    }
}
