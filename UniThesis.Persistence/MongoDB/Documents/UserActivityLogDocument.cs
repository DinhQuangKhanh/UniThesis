using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniThesis.Persistence.MongoDB.Documents
{
    /// <summary>
    /// Document for user activity logs.
    /// Tracks all actions performed by users across the system, categorized by role.
    /// </summary>
    public class UserActivityLogDocument
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>ID of the user who performed the action.</summary>
        public Guid UserId { get; set; }

        /// <summary>Display name of the user at the time of the action.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Email of the user at the time of the action.</summary>
        public string? UserEmail { get; set; }

        /// <summary>
        /// The role context under which the action was performed
        /// (e.g. "admin", "mentor", "evaluator", "student").
        /// </summary>
        public string UserRole { get; set; } = string.Empty;

        /// <summary>
        /// Semantic action name (e.g. "MemberRemoved", "ProjectSubmitted", "EvaluationCompleted").
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Category to group related actions for easier filtering
        /// (e.g. "Group", "Project", "Evaluation", "Meeting", "Defense", "System").
        /// </summary>
        public string? Category { get; set; }

        /// <summary>Type of entity involved (e.g. "Group", "Project").</summary>
        public string? EntityType { get; set; }

        /// <summary>ID of the entity involved.</summary>
        public Guid? EntityId { get; set; }

        /// <summary>
        /// Severity level: "info" (default), "warning", "error", "critical".
        /// Helps admins filter important actions quickly.
        /// </summary>
        [BsonDefaultValue("info")]
        public string Severity { get; set; } = "info";

        /// <summary>Client IP address.</summary>
        public string? IpAddress { get; set; }

        /// <summary>Client User-Agent string.</summary>
        public string? UserAgent { get; set; }

        /// <summary>When the action occurred (UTC).</summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>Flexible key-value bag for action-specific data.</summary>
        [BsonExtraElements]
        public BsonDocument? Details { get; set; }
    }
}
