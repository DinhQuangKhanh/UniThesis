using UniThesis.Domain.Aggregates.MeetingAggregate.Events;
using UniThesis.Domain.Aggregates.MeetingAggregate.ValueObjects;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Enums.Meeting;

namespace UniThesis.Domain.Aggregates.MeetingAggregate
{
    public class MeetingSchedule : AggregateRoot<Guid>
    {
        public Guid GroupId { get; private set; }
        public Guid MentorId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public int? DurationMinutes { get; private set; }
        public MeetingLocation Location { get; private set; } = null!;
        public MeetingStatus Status { get; private set; }
        public Guid RequestedBy { get; private set; }
        public string? Notes { get; private set; }
        public string? RejectionReason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private MeetingSchedule() { }

        public static MeetingSchedule Create(
            Guid groupId,
            Guid mentorId,
            string title,
            DateTime scheduledDate,
            MeetingLocation location,
            Guid requestedBy,
            string? description = null,
            int? durationMinutes = 60)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            var meeting = new MeetingSchedule
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                MentorId = mentorId,
                Title = title.Trim(),
                Description = description?.Trim(),
                ScheduledDate = scheduledDate,
                DurationMinutes = durationMinutes,
                Location = location,
                Status = MeetingStatus.Pending,
                RequestedBy = requestedBy,
                CreatedAt = DateTime.UtcNow
            };

            meeting.RaiseDomainEvent(new MeetingRequestedEvent(meeting.Id, groupId, mentorId));
            return meeting;
        }

        public void Approve()
        {
            if (Status != MeetingStatus.Pending)
                throw new BusinessRuleValidationException("Only pending meetings can be approved.");

            Status = MeetingStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new MeetingApprovedEvent(Id));
        }

        public void Reject(string? reason = null)
        {
            if (Status != MeetingStatus.Pending)
                throw new BusinessRuleValidationException("Only pending meetings can be rejected.");

            Status = MeetingStatus.Rejected;
            RejectionReason = reason;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new MeetingRejectedEvent(Id, reason));
        }

        public void Complete()
        {
            if (Status != MeetingStatus.Approved)
                throw new BusinessRuleValidationException("Only approved meetings can be completed.");

            Status = MeetingStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new MeetingCompletedEvent(Id));
        }

        public void Cancel()
        {
            if (Status == MeetingStatus.Completed)
                throw new BusinessRuleValidationException("Completed meetings cannot be cancelled.");

            Status = MeetingStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new MeetingCancelledEvent(Id));
        }

        public void Reschedule(DateTime newDate)
        {
            if (Status == MeetingStatus.Completed || Status == MeetingStatus.Cancelled)
                throw new BusinessRuleValidationException("Cannot reschedule completed or cancelled meetings.");

            ScheduledDate = newDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLocation(MeetingLocation location)
        {
            Location = location;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string? title = null, string? description = null, int? durationMinutes = null)
        {
            if (Status == MeetingStatus.Completed || Status == MeetingStatus.Cancelled)
                throw new BusinessRuleValidationException("Cannot update completed or cancelled meetings.");

            if (!string.IsNullOrWhiteSpace(title))
                Title = title.Trim();

            if (description != null)
                Description = description.Trim();

            if (durationMinutes.HasValue)
                DurationMinutes = durationMinutes.Value;

            UpdatedAt = DateTime.UtcNow;
        }

        public void SetNotes(string? notes)
        {
            Notes = notes;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
