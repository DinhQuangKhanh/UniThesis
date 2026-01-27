using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UniThesis.Domain.Enums.Evaluation;

namespace UniThesis.Persistence.MongoDB.Documents
{
    /// <summary>
    /// Document for evaluation audit logs.
    /// </summary>
    public class EvaluationLogDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public Guid? EvaluationSubmissionId { get; set; }

        public int SubmissionNumber { get; set; }

        [BsonRepresentation(BsonType.String)]
        public EvaluationAction Action { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? PerformedBy { get; set; }

        public string PerformedByName { get; set; } = string.Empty;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

        [BsonRepresentation(BsonType.String)]
        public EvaluationResult? Result { get; set; }

        public string? Feedback { get; set; }
        public string? CriteriaFeedbackJson { get; set; }
        public string? ProjectSnapshotJson { get; set; }

        [BsonExtraElements]
        public BsonDocument? Metadata { get; set; }
    }
}
