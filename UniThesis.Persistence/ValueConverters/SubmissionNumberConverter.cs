using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.EvaluationAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts SubmissionNumber to/from int for database storage.
    /// </summary>
    public class SubmissionNumberConverter : ValueConverter<SubmissionNumber, int>
    {
        public SubmissionNumberConverter()
            : base(
                number => number.Value,
                value => SubmissionNumber.Create(value))
        { }
    }
}
