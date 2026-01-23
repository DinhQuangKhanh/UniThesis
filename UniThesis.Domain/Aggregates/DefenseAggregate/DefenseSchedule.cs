using UniThesis.Domain.Aggregates.DefenseAggregate.Entities;
using UniThesis.Domain.Aggregates.DefenseAggregate.Entities.ValueObjects;
using UniThesis.Domain.Aggregates.DefenseAggregate.Events;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Enums.Defense;

namespace UniThesis.Domain.Aggregates.DefenseAggregate
{
    public class DefenseSchedule : AggregateRoot<Guid>
    {
        public Guid GroupId { get; private set; }
        public int? CouncilId { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public DefenseLocation Location { get; private set; } = null!;
        public int? DurationMinutes { get; private set; }
        public DefenseScheduleStatus Status { get; private set; }
        public string? Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public DefenseCouncil? Council { get; private set; }

        private DefenseSchedule() { }

        public static DefenseSchedule Create(Guid groupId, DateTime scheduledDate, DefenseLocation location, int? durationMinutes = 60)
        {
            var defense = new DefenseSchedule
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                ScheduledDate = scheduledDate,
                Location = location,
                DurationMinutes = durationMinutes,
                Status = DefenseScheduleStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };
            defense.RaiseDomainEvent(new DefenseScheduledEvent(defense.Id, groupId, scheduledDate));
            return defense;
        }

        public void AssignCouncil(DefenseCouncil council)
        {
            Council = council;
            CouncilId = council.Id;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Start()
        {
            if (Status != DefenseScheduleStatus.Scheduled)
                throw new BusinessRuleValidationException("Only scheduled defenses can be started.");
            Status = DefenseScheduleStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != DefenseScheduleStatus.InProgress)
                throw new BusinessRuleValidationException("Only in-progress defenses can be completed.");
            Status = DefenseScheduleStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new DefenseCompletedEvent(Id, GroupId));
        }

        public void Postpone(DateTime newDate)
        {
            if (Status == DefenseScheduleStatus.Completed)
                throw new BusinessRuleValidationException("Completed defenses cannot be postponed.");
            ScheduledDate = newDate;
            Status = DefenseScheduleStatus.Postponed;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new DefensePostponedEvent(Id, newDate));
        }

        public void Cancel()
        {
            if (Status == DefenseScheduleStatus.Completed)
                throw new BusinessRuleValidationException("Completed defenses cannot be cancelled.");
            Status = DefenseScheduleStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new DefenseCancelledEvent(Id));
        }

        public void UpdateLocation(DefenseLocation location) { Location = location; UpdatedAt = DateTime.UtcNow; }
        public void SetNotes(string? notes) { Notes = notes; UpdatedAt = DateTime.UtcNow; }
    }
}
