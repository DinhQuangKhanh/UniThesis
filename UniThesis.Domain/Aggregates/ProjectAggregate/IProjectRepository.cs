using UniThesis.Domain.Aggregates.ProjectAggregate.ValueObjects;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Domain.Aggregates.ProjectAggregate
{
    public interface IProjectRepository : IRepository<Project, Guid>
    {
        /// <summary>
        /// Gets a project by its code.
        /// </summary>
        Task<Project?> GetByCodeAsync(ProjectCode code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a project with its mentors loaded.
        /// </summary>
        Task<Project?> GetWithMentorsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a project with its documents loaded.
        /// </summary>
        Task<Project?> GetWithDocumentsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a project with all related entities loaded.
        /// </summary>
        Task<Project?> GetWithAllAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all projects by mentor identifier.
        /// </summary>
        Task<IEnumerable<Project>> GetByMentorIdAsync(Guid mentorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all projects by semester identifier.
        /// </summary>
        Task<IEnumerable<Project>> GetBySemesterIdAsync(int semesterId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all projects by status.
        /// </summary>
        Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all projects pending evaluation.
        /// </summary>
        Task<IEnumerable<Project>> GetPendingEvaluationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all projects by group identifier.
        /// </summary>
        Task<Project?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all projects by major identifier.
        /// </summary>
        Task<IEnumerable<Project>> GetByMajorIdAsync(int majorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a project code already exists.
        /// </summary>
        Task<bool> ExistsCodeAsync(ProjectCode code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the next sequence number for generating project codes.
        /// </summary>
        Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of projects by status for a semester.
        /// </summary>
        Task<Dictionary<ProjectStatus, int>> GetStatusCountBySemesterAsync(int semesterId, CancellationToken cancellationToken = default);
    }
}
