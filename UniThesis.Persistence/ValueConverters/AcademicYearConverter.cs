using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.SemesterAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts AcademicYear to/from string for database storage.
    /// </summary>
    public class AcademicYearConverter : ValueConverter<AcademicYear, string>
    {
        public AcademicYearConverter()
            : base(
                year => year.Value,
                value => AcademicYear.Parse(value))
        { }
    }
}
