using UniThesis.Domain.Common.Primitives;

namespace UniThesis.Domain.Aggregates.EvaluationAggregate.ValueObjects
{
    public sealed class ProjectSnapshot : ValueObject
    {
        public string NameVi { get; }
        public string Description { get; }
        public string Objectives { get; }
        public string? Scope { get; }
        public string? Technologies { get; }
        public string? ExpectedResults { get; }
        public DateTime CapturedAt { get; }

        private ProjectSnapshot(string nameVi, string description, string objectives,
            string? scope, string? technologies, string? expectedResults, DateTime capturedAt)
        {
            NameVi = nameVi;
            Description = description;
            Objectives = objectives;
            Scope = scope;
            Technologies = technologies;
            ExpectedResults = expectedResults;
            CapturedAt = capturedAt;
        }

        public static ProjectSnapshot Capture(string nameVi, string description, string objectives,
            string? scope, string? technologies, string? expectedResults)
        {
            return new ProjectSnapshot(nameVi, description, objectives, scope, technologies, expectedResults, DateTime.UtcNow);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return NameVi;
            yield return Description;
            yield return Objectives;
            yield return Scope;
            yield return Technologies;
            yield return ExpectedResults;
            yield return CapturedAt;
        }
    }
}
