using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.GroupAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts GroupCode to/from string for database storage.
    /// </summary>
    public class GroupCodeConverter : ValueConverter<GroupCode, string>
    {
        public GroupCodeConverter()
            : base(
                code => code.Value,
                value => GroupCode.Create(value))
        { }
    }
}
