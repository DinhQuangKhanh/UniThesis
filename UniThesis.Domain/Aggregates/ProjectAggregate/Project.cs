using UniThesis.Domain.Aggregates.ProjectAggregate.Entities;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Domain.Aggregates.ProjectAggregate.Rules;
using UniThesis.Domain.Aggregates.ProjectAggregate.ValueObjects;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Common.Rules;
using UniThesis.Domain.Enums.Document;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Domain.Enums.Mentor;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Domain.Aggregates.ProjectAggregate
{
    public class Project : AggregateRoot<Guid>
    {
        private readonly List<ProjectMentor> _mentors = new();
        private readonly List<Document> _documents = new();

        #region Properties

        public ProjectCode Code { get; private set; } = null!;
        public ProjectName NameVi { get; private set; } = null!;
        public ProjectName? NameEn { get; private set; }
        public string? NameAbbr { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public string Objectives { get; private set; } = string.Empty;
        public string? Scope { get; private set; }
        public TechnologyStack? Technologies { get; private set; }
        public string? ExpectedResults { get; private set; }
        public int MajorId { get; private set; }
        public int SemesterId { get; private set; }
        public Guid? GroupId { get; private set; }
        public Guid? TopicPoolId { get; private set; }
        public int MaxStudents { get; private set; }
        public ProjectSourceType SourceType { get; private set; }
        public RegistrationType RegistrationType { get; private set; }
        public ProjectStatus Status { get; private set; }
        public ProjectPriority Priority { get; private set; }
        public DateTime? SubmittedAt { get; private set; }
        public Guid? SubmittedBy { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? Deadline { get; private set; }
        public int EvaluationCount { get; private set; }
        public EvaluationResult? LastEvaluationResult { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<ProjectMentor> Mentors => _mentors.AsReadOnly();
        public IReadOnlyCollection<Document> Documents => _documents.AsReadOnly();
        public IReadOnlyCollection<ProjectMentor> ActiveMentors => _mentors.Where(m => m.IsActive).ToList().AsReadOnly();
        public ProjectMentor? PrimaryMentor => _mentors.FirstOrDefault(m => m.Role == MentorRole.Primary && m.IsActive);
        public ProjectMentor? SecondaryMentor => _mentors.FirstOrDefault(m => m.Role == MentorRole.Secondary && m.IsActive);

        #endregion

        #region Constructors

        private Project() { }

        private Project(Guid id, ProjectCode code, ProjectName nameVi, string description, string objectives,
            int majorId, int semesterId, int maxStudents, ProjectSourceType sourceType, Guid? topicPoolId = null) : base(id)
        {
            Code = code;
            NameVi = nameVi;
            Description = description;
            Objectives = objectives;
            MajorId = majorId;
            SemesterId = semesterId;
            MaxStudents = maxStudents;
            SourceType = sourceType;
            TopicPoolId = topicPoolId;
            Status = ProjectStatus.Draft;
            Priority = ProjectPriority.Normal;
            RegistrationType = RegistrationType.Public;
            EvaluationCount = 0;
            CreatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Factory Methods

        public static Project CreateFromPool(ProjectCode code, ProjectName nameVi, string description, string objectives,
            int majorId, int semesterId, int maxStudents, Guid topicPoolId)
        {
            var project = new Project(Guid.NewGuid(), code, nameVi, description, objectives, majorId, semesterId, maxStudents, ProjectSourceType.FromPool, topicPoolId);
            project.RaiseDomainEvent(new ProjectCreatedEvent(project.Id, project.Code.Value, ProjectSourceType.FromPool));
            return project;
        }

        public static Project CreateDirect(ProjectCode code, ProjectName nameVi, string description, string objectives,
            int majorId, int semesterId, int maxStudents)
        {
            var project = new Project(Guid.NewGuid(), code, nameVi, description, objectives, majorId, semesterId, maxStudents, ProjectSourceType.DirectRegistration);
            project.RaiseDomainEvent(new ProjectCreatedEvent(project.Id, project.Code.Value, ProjectSourceType.DirectRegistration));
            return project;
        }

        #endregion

        #region Mentor Management

        public void AddMentor(Guid mentorId, MentorRole role, Guid assignedBy)
        {
            CheckRule(new ProjectCannotExceedMaxMentorsRule(ActiveMentors.Count));
            if (_mentors.Any(m => m.MentorId == mentorId && m.IsActive))
                throw new BusinessRuleValidationException("Mentor is already assigned to this project.");

            var mentor = ProjectMentor.Create(Id, mentorId, role, assignedBy);
            _mentors.Add(mentor);
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new MentorAssignedEvent(Id, mentorId, role));
        }

        public void RemoveMentor(Guid mentorId)
        {
            var mentor = _mentors.FirstOrDefault(m => m.MentorId == mentorId && m.IsActive)
                ?? throw new EntityNotFoundException(nameof(ProjectMentor), mentorId);
            if (mentor.Role == MentorRole.Primary)
                throw new BusinessRuleValidationException("Cannot remove primary mentor.");
            mentor.Deactivate();
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new MentorRemovedEvent(Id, mentorId));
        }

        #endregion

        #region Evaluation Workflow

        public void SubmitForEvaluation(Guid submittedBy)
        {
            CheckRule(new ProjectCanOnlyBeSubmittedWhenDraftRule(Status));
            Status = ProjectStatus.PendingEvaluation;
            SubmittedAt = DateTime.UtcNow;
            SubmittedBy = submittedBy;
            EvaluationCount++;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectSubmittedEvent(Id, submittedBy, EvaluationCount));
        }

        public void Approve()
        {
            if (Status != ProjectStatus.PendingEvaluation)
                throw new BusinessRuleValidationException("Only projects pending evaluation can be approved.");
            Status = ProjectStatus.Approved;
            ApprovedAt = DateTime.UtcNow;
            LastEvaluationResult = EvaluationResult.Approved;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectApprovedEvent(Id));
        }

        public void RequestModification()
        {
            if (Status != ProjectStatus.PendingEvaluation)
                throw new BusinessRuleValidationException("Only projects pending evaluation can request modification.");
            Status = ProjectStatus.NeedsModification;
            LastEvaluationResult = EvaluationResult.NeedsModification;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectModificationRequestedEvent(Id));
        }

        public void Reject()
        {
            if (Status != ProjectStatus.PendingEvaluation)
                throw new BusinessRuleValidationException("Only projects pending evaluation can be rejected.");
            Status = ProjectStatus.Rejected;
            LastEvaluationResult = EvaluationResult.Rejected;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectRejectedEvent(Id));
        }

        public void UpdateAfterFeedback(string? description = null, string? objectives = null, string? scope = null,
            string? technologies = null, string? expectedResults = null)
        {
            CheckRule(new ProjectCanOnlyBeModifiedWhenNeedsModificationRule(Status));
            if (!string.IsNullOrWhiteSpace(description)) Description = description;
            if (!string.IsNullOrWhiteSpace(objectives)) Objectives = objectives;
            if (scope != null) Scope = scope;
            if (!string.IsNullOrWhiteSpace(technologies)) Technologies = TechnologyStack.Create(technologies);
            if (expectedResults != null) ExpectedResults = expectedResults;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectModifiedEvent(Id));
        }

        public void Resubmit(Guid submittedBy)
        {
            CheckRule(new ProjectCanOnlyBeSubmittedWhenDraftRule(Status, allowNeedsModification: true));
            Status = ProjectStatus.PendingEvaluation;
            SubmittedAt = DateTime.UtcNow;
            SubmittedBy = submittedBy;
            EvaluationCount++;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectResubmittedEvent(Id, submittedBy, EvaluationCount));
        }

        #endregion

        #region Project Lifecycle

        public void AssignGroup(Guid groupId)
        {
            GroupId = groupId;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectGroupAssignedEvent(Id, groupId));
        }

        public void StartProgress(DateTime startDate, DateTime deadline)
        {
            if (Status != ProjectStatus.Approved)
                throw new BusinessRuleValidationException("Only approved projects can start progress.");
            if (deadline <= startDate)
                throw new BusinessRuleValidationException("Deadline must be after start date.");
            Status = ProjectStatus.InProgress;
            StartDate = startDate;
            Deadline = deadline;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectStartedEvent(Id, startDate, deadline));
        }

        public void Complete()
        {
            if (Status != ProjectStatus.InProgress)
                throw new BusinessRuleValidationException("Only in-progress projects can be completed.");
            Status = ProjectStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectCompletedEvent(Id));
        }

        public void Cancel()
        {
            if (Status == ProjectStatus.Completed)
                throw new BusinessRuleValidationException("Completed projects cannot be cancelled.");
            Status = ProjectStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProjectCancelledEvent(Id));
        }

        #endregion

        #region Document Management

        public Document AddDocument(string fileName, string originalFileName, string fileType, long fileSize,
            string filePath, DocumentType documentType, Guid uploadedBy)
        {
            var document = Document.Create(Id, fileName, originalFileName, fileType, fileSize, filePath, documentType, uploadedBy);
            _documents.Add(document);
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new DocumentUploadedEvent(Id, document.Id, documentType));
            return document;
        }

        public void RemoveDocument(Guid documentId)
        {
            var document = _documents.FirstOrDefault(d => d.Id == documentId && !d.IsDeleted)
                ?? throw new EntityNotFoundException(nameof(Document), documentId);
            document.Delete();
            UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Update Methods

        public void UpdateBasicInfo(ProjectName? nameVi = null, ProjectName? nameEn = null, string? nameAbbr = null,
            string? description = null, string? objectives = null, string? scope = null, string? technologies = null, string? expectedResults = null)
        {
            if (Status != ProjectStatus.Draft && Status != ProjectStatus.NeedsModification)
                throw new BusinessRuleValidationException("Project can only be updated when in Draft or NeedsModification status.");
            if (nameVi != null) NameVi = nameVi;
            if (nameEn != null) NameEn = nameEn;
            if (nameAbbr != null) NameAbbr = nameAbbr;
            if (!string.IsNullOrWhiteSpace(description)) Description = description;
            if (!string.IsNullOrWhiteSpace(objectives)) Objectives = objectives;
            if (scope != null) Scope = scope;
            if (!string.IsNullOrWhiteSpace(technologies)) Technologies = TechnologyStack.Create(technologies);
            if (expectedResults != null) ExpectedResults = expectedResults;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPriority(ProjectPriority priority) { Priority = priority; UpdatedAt = DateTime.UtcNow; }
        public void SetMaxStudents(int maxStudents)
        {
            if (maxStudents < 1 || maxStudents > 5)
                throw new BusinessRuleValidationException("Maximum students must be between 1 and 5.");
            MaxStudents = maxStudents;
            UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        private void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleValidationException(rule);
        }
    }
}
