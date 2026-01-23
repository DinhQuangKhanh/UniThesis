using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts TopicCode to/from string for database storage.
    /// </summary>
    public class TopicCodeConverter : ValueConverter<TopicCode, string>
    {
        public TopicCodeConverter()
            : base(
                code => code.Value,
                value => TopicCode.Create(value))
        { }
    }
}
