using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.SemesterAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts SemesterCode to/from string for database storage.
    /// </summary>
    public class SemesterCodeConverter : ValueConverter<SemesterCode, string>
    {
        public SemesterCodeConverter()
            : base(
                code => code.Value,
                value => SemesterCode.Create(value))
        { }
    }
}
