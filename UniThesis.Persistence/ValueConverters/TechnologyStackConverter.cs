using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.ProjectAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts TechnologyStack to/from string for database storage.
    /// </summary>
    public class TechnologyStackConverter : ValueConverter<TechnologyStack?, string?>
    {
        public TechnologyStackConverter()
            : base(
                stack => stack == null ? null : stack.Value,
                value => string.IsNullOrEmpty(value) ? null : TechnologyStack.Create(value))
        { }
    }

}
