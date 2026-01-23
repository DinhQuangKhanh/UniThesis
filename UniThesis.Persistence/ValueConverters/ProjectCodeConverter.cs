using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.ProjectAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts ProjectCode to/from string for database storage.
    /// </summary>
    public class ProjectCodeConverter : ValueConverter<ProjectCode, string>
    {
        public ProjectCodeConverter()
            : base(
                code => code.Value,
                value => ProjectCode.Create(value))
        { }
    }
}
