using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniThesis.Domain.Aggregates.SupportAggregate.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts TicketCode to/from string for database storage.
    /// </summary>
    public class TicketCodeConverter : ValueConverter<TicketCode, string>
    {
        public TicketCodeConverter()
            : base(
                code => code.Value,
                value => TicketCode.Create(value))
        { }
    }
}
