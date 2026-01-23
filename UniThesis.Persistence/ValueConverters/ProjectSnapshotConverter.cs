using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using UniThesis.Domain.Aggregates.EvaluationAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts ProjectSnapshot to/from JSON string for database storage.
    /// </summary>
    public class ProjectSnapshotConverter : ValueConverter<ProjectSnapshot?, string?>
    {
        public ProjectSnapshotConverter()
            : base(
                snapshot => snapshot == null ? null : JsonSerializer.Serialize(snapshot, JsonOptions),
                json => string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<ProjectSnapshot>(json, JsonOptions))
        { }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
