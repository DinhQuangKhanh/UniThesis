using UniThesis.Domain.Common.Primitives;

namespace UniThesis.Domain.Aggregates.SupportAggregate
{
    /// <summary>
    /// Represents a single message or reply within a support ticket.
    /// </summary>
    public class TicketMessage : Entity<Guid>
    {
        public Guid TicketId { get; private set; }
        public Guid SenderId { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        private TicketMessage() { } // EF Core

        internal static TicketMessage Create(Guid ticketId, Guid senderId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content cannot be empty.", nameof(content));

            return new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                SenderId = senderId,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
