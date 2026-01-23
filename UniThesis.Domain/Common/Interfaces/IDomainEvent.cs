using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniThesis.Domain.Common.Interfaces
{
    public interface IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier of the event.
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// Gets the date and time when the event occurred.
        /// </summary>
        DateTime OccurredOn { get; }
    }

    /// <summary>
    /// Base record for domain events providing default implementations.
    /// </summary>
    public abstract record DomainEventBase : IDomainEvent
    {
        /// <inheritdoc />
        public Guid EventId { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
