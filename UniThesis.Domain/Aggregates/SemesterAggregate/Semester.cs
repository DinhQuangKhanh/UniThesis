using UniThesis.Domain.Aggregates.SemesterAggregate.Entities;
using UniThesis.Domain.Aggregates.SemesterAggregate.Events;
using UniThesis.Domain.Aggregates.SemesterAggregate.Rules;
using UniThesis.Domain.Aggregates.SemesterAggregate.ValueObjects;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Common.Rules;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Domain.Aggregates.SemesterAggregate
{
    public class Semester : AggregateRoot<int>
    {
        private readonly List<SemesterPhase> _phases = new();

        public string Name { get; private set; } = string.Empty;
        public SemesterCode Code { get; private set; } = null!;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public SemesterStatus Status { get; private set; }
        public AcademicYear AcademicYear { get; private set; } = null!;
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<SemesterPhase> Phases => _phases.AsReadOnly();
        public SemesterPhase? CurrentPhase => _phases.FirstOrDefault(p => p.IsCurrent);
        public bool IsActive => Status == SemesterStatus.Active;

        private Semester() { }

        public static Semester Create(int id, string name, SemesterCode code, DateTime startDate, DateTime endDate,
            AcademicYear academicYear, string? description = null)
        {
            if (endDate <= startDate)
                throw new ArgumentException("End date must be after start date.");

            var semester = new Semester
            {
                Id = id,
                Name = name,
                Code = code,
                StartDate = startDate,
                EndDate = endDate,
                AcademicYear = academicYear,
                Description = description,
                Status = SemesterStatus.Upcoming,
                CreatedAt = DateTime.UtcNow
            };

            semester.RaiseDomainEvent(new SemesterCreatedEvent(id, code.Value));
            return semester;
        }

        public SemesterPhase AddPhase(string name, SemesterPhaseType type, DateTime startDate, DateTime endDate)
        {
            var existingPhases = _phases.Select(p => (p.StartDate, p.EndDate));
            CheckRule(new PhasesMustNotOverlapRule(existingPhases, startDate, endDate));

            if (startDate < StartDate || endDate > EndDate)
                throw new BusinessRuleValidationException("Phase dates must be within semester dates.");

            var order = _phases.Count + 1;
            var phase = SemesterPhase.Create(Id, name, type, startDate, endDate, order);
            _phases.Add(phase);
            UpdatedAt = DateTime.UtcNow;
            return phase;
        }

        public void Activate()
        {
            if (Status != SemesterStatus.Upcoming)
                throw new BusinessRuleValidationException("Only upcoming semesters can be activated.");
            Status = SemesterStatus.Active;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new SemesterActivatedEvent(Id));
        }

        public void Close()
        {
            if (Status != SemesterStatus.Active)
                throw new BusinessRuleValidationException("Only active semesters can be closed.");
            Status = SemesterStatus.Closed;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new SemesterClosedEvent(Id));
        }

        public void StartPhase(int phaseId)
        {
            var phase = _phases.FirstOrDefault(p => p.Id == phaseId)
                ?? throw new EntityNotFoundException(nameof(SemesterPhase), phaseId);

            if (CurrentPhase != null)
                throw new BusinessRuleValidationException("Complete current phase before starting a new one.");

            phase.Start();
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new PhaseStartedEvent(Id, phaseId, phase.Type));
        }

        public void CompletePhase(int phaseId)
        {
            var phase = _phases.FirstOrDefault(p => p.Id == phaseId)
                ?? throw new EntityNotFoundException(nameof(SemesterPhase), phaseId);
            phase.Complete();
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new PhaseCompletedEvent(Id, phaseId, phase.Type));
        }

        public void UpdateDates(DateTime startDate, DateTime endDate)
        {
            CheckRule(new SemesterDatesMustBeValidRule(startDate, endDate));
            StartDate = startDate;
            EndDate = endDate;
            UpdatedAt = DateTime.UtcNow;
        }

        private void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleValidationException(rule);
        }
    }
}
