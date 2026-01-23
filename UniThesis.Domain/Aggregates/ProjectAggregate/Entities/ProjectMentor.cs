using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Enums.Mentor;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Entities
{
    public class ProjectMentor : Entity<int>
    {
        /// <summary>
        /// Gets the project identifier.
        /// </summary>
        public Guid ProjectId { get; private set; }

        /// <summary>
        /// Gets the mentor (user) identifier.
        /// </summary>
        public Guid MentorId { get; private set; }

        /// <summary>
        /// Gets the role of the mentor (Primary or Secondary).
        /// </summary>
        public MentorRole Role { get; private set; }

        /// <summary>
        /// Gets the status of the mentor assignment.
        /// </summary>
        public ProjectMentorStatus Status { get; private set; }

        /// <summary>
        /// Gets the date when the mentor was assigned.
        /// </summary>
        public DateTime AssignedAt { get; private set; }

        /// <summary>
        /// Gets the identifier of the user who assigned this mentor.
        /// </summary>
        public Guid? AssignedBy { get; private set; }

        /// <summary>
        /// Gets any additional notes about the mentor assignment.
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the mentor assignment is active.
        /// </summary>
        public bool IsActive => Status == ProjectMentorStatus.Active;

        private ProjectMentor() { }

        /// <summary>
        /// Creates a new mentor assignment for a project.
        /// </summary>
        internal static ProjectMentor Create(
            Guid projectId,
            Guid mentorId,
            MentorRole role,
            Guid assignedBy)
        {
            return new ProjectMentor
            {
                ProjectId = projectId,
                MentorId = mentorId,
                Role = role,
                Status = ProjectMentorStatus.Active,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            };
        }

        /// <summary>
        /// Deactivates the mentor assignment.
        /// </summary>
        public void Deactivate()
        {
            Status = ProjectMentorStatus.Inactive;
        }

        /// <summary>
        /// Reactivates the mentor assignment.
        /// </summary>
        public void Reactivate()
        {
            Status = ProjectMentorStatus.Active;
        }

        /// <summary>
        /// Changes the role of the mentor.
        /// </summary>
        /// <param name="newRole">The new role.</param>
        public void ChangeRole(MentorRole newRole)
        {
            Role = newRole;
        }

        /// <summary>
        /// Sets notes for the mentor assignment.
        /// </summary>
        /// <param name="notes">The notes.</param>
        public void SetNotes(string? notes)
        {
            Notes = notes;
        }
    }
}
