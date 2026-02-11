using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UniThesis.Domain.Enums.Message;

namespace UniThesis.Persistence.MongoDB.Documents
{
    /// <summary>
    /// Document for messages.
    /// </summary>
    public class MessageDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid ConversationId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid SenderId { get; set; }

        public string SenderName { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public MessageType Type { get; set; }

        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }

        [BsonRepresentation(BsonType.String)]
        public List<Guid> ReadByUserIds { get; set; } = [];

        public bool IsDeleted { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? EditedAt { get; set; }
    }
}
