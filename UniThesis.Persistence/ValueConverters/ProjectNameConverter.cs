using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.ProjectAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts ProjectName to/from string for database storage.
    /// </summary>
    public class ProjectNameConverter : ValueConverter<ProjectName, string>
    {
        public ProjectNameConverter()
            : base(
                name => name.Value,
                value => ProjectName.Create(value))
        { }
    }
}
