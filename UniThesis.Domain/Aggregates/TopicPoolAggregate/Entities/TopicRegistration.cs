using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Entities
{
    public class TopicRegistration : Entity<Guid>
    {
        public Guid TopicPoolId { get; private set; }
        public Guid GroupId { get; private set; }
        public Guid RegisteredBy { get; private set; }
        public DateTime RegisteredAt { get; private set; }
        public TopicRegistrationStatus Status { get; private set; }
        public Guid? ConfirmedBy { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public string? CancelledReason { get; private set; }
        public string? Notes { get; private set; }

        private TopicRegistration() { }

        internal static TopicRegistration Create(Guid topicPoolId, Guid groupId, Guid registeredBy)
        {
            return new TopicRegistration
            {
                Id = Guid.NewGuid(),
                TopicPoolId = topicPoolId,
                GroupId = groupId,
                RegisteredBy = registeredBy,
                RegisteredAt = DateTime.UtcNow,
                Status = TopicRegistrationStatus.Pending
            };
        }

        public void Confirm(Guid confirmedBy)
        {
            Status = TopicRegistrationStatus.Confirmed;
            ConfirmedBy = confirmedBy;
            ConfirmedAt = DateTime.UtcNow;
        }

        public void Cancel(string? reason = null)
        {
            Status = TopicRegistrationStatus.Cancelled;
            CancelledReason = reason;
        }

        public void SetNotes(string? notes) => Notes = notes;
    }
}
