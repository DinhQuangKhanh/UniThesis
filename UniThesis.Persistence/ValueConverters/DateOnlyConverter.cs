using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts DateOnly to/from DateTime for SQL Server storage.
    /// </summary>
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter()
            : base(
                dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime))
        { }
    }
}
