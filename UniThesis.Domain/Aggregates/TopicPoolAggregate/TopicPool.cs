using UniThesis.Domain.Aggregates.TopicPoolAggregate.Entities;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Rules;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.ValueObjects;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Common.Rules;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate
{
    public class TopicPool : AggregateRoot<Guid>
    {
        private readonly List<TopicRegistration> _registrations = new();

        public TopicCode Code { get; private set; } = null!;
        public string NameVi { get; private set; } = string.Empty;
        public string? NameEn { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public string Objectives { get; private set; } = string.Empty;
        public string? Scope { get; private set; }
        public string? Technologies { get; private set; }
        public string? ExpectedResults { get; private set; }
        public int MajorId { get; private set; }
        public Guid ProposedBy { get; private set; }
        public int MaxStudents { get; private set; }
        public int CreatedSemesterId { get; private set; }
        public int ExpirationSemesterId { get; private set; }
        public TopicPoolStatus Status { get; private set; }
        public Guid? SelectedByGroupId { get; private set; }
        public DateTime? SelectedAt { get; private set; }
        public Guid? ConvertedToProjectId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<TopicRegistration> Registrations => _registrations.AsReadOnly();

        private TopicPool() { }

        public static TopicPool Create(TopicCode code, string nameVi, string description, string objectives,
            int majorId, Guid proposedBy, int maxStudents, int createdSemesterId, int expirationSemesterId)
        {
            var topic = new TopicPool
            {
                Id = Guid.NewGuid(),
                Code = code,
                NameVi = nameVi,
                Description = description,
                Objectives = objectives,
                MajorId = majorId,
                ProposedBy = proposedBy,
                MaxStudents = maxStudents,
                CreatedSemesterId = createdSemesterId,
                ExpirationSemesterId = expirationSemesterId,
                Status = TopicPoolStatus.Available,
                CreatedAt = DateTime.UtcNow
            };
            topic.RaiseDomainEvent(new TopicCreatedEvent(topic.Id, proposedBy));
            return topic;
        }

        public TopicRegistration RequestRegistration(Guid groupId, Guid registeredBy)
        {
            CheckRule(new TopicMustBeAvailableForSelectionRule(Status));
            if (_registrations.Any(r => r.GroupId == groupId && r.Status == TopicRegistrationStatus.Pending))
                throw new BusinessRuleValidationException("Group already has a pending registration.");

            var registration = TopicRegistration.Create(Id, groupId, registeredBy);
            _registrations.Add(registration);
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new RegistrationRequestedEvent(Id, groupId, registeredBy));
            return registration;
        }

        public void ConfirmRegistration(Guid registrationId, Guid confirmedBy)
        {
            var registration = _registrations.FirstOrDefault(r => r.Id == registrationId)
                ?? throw new EntityNotFoundException(nameof(TopicRegistration), registrationId);

            registration.Confirm(confirmedBy);
            Status = TopicPoolStatus.Selected;
            SelectedByGroupId = registration.GroupId;
            SelectedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            foreach (var other in _registrations.Where(r => r.Id != registrationId && r.Status == TopicRegistrationStatus.Pending))
                other.Cancel("Another group was selected");

            RaiseDomainEvent(new RegistrationConfirmedEvent(Id, registration.GroupId));
        }

        public void CancelRegistration(Guid registrationId, string? reason = null)
        {
            var registration = _registrations.FirstOrDefault(r => r.Id == registrationId)
                ?? throw new EntityNotFoundException(nameof(TopicRegistration), registrationId);
            registration.Cancel(reason);
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new RegistrationCancelledEvent(Id, registration.GroupId, reason));
        }

        public void MarkAsExpired()
        {
            if (Status != TopicPoolStatus.Available)
                throw new BusinessRuleValidationException("Only available topics can be marked as expired.");
            Status = TopicPoolStatus.Expired;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new TopicExpiredEvent(Id));
        }

        public void Archive()
        {
            Status = TopicPoolStatus.Archived;
            UpdatedAt = DateTime.UtcNow;
        }

        public void LinkToProject(Guid projectId)
        {
            ConvertedToProjectId = projectId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string? nameVi = null, string? nameEn = null, string? description = null,
            string? objectives = null, string? scope = null, string? technologies = null, string? expectedResults = null)
        {
            if (Status != TopicPoolStatus.Available)
                throw new BusinessRuleValidationException("Only available topics can be updated.");
            if (!string.IsNullOrWhiteSpace(nameVi)) NameVi = nameVi;
            if (nameEn != null) NameEn = nameEn;
            if (!string.IsNullOrWhiteSpace(description)) Description = description;
            if (!string.IsNullOrWhiteSpace(objectives)) Objectives = objectives;
            if (scope != null) Scope = scope;
            if (technologies != null) Technologies = technologies;
            if (expectedResults != null) ExpectedResults = expectedResults;
            UpdatedAt = DateTime.UtcNow;
        }

        private void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleValidationException(rule);
        }
    }
}
