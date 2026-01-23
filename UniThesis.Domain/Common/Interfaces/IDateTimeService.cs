

namespace UniThesis.Domain.Common.Interfaces
{
    public interface IDateTimeService
    {
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current local date and time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the current date (without time component).
        /// </summary>
        DateOnly Today { get; }
    }
}
