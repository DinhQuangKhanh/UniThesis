using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Common.Interfaces;

/// <summary>
/// Read-only query service for complex StudentGroup queries that span multiple aggregates.
/// Implemented in the Persistence layer with direct DbContext access for optimal performance.
/// </summary>
public interface IStudentGroupQueryService
{
    /// <summary>
    /// Gets all groups that a mentor is guiding in a specific semester.
    /// If semesterId is null, uses the currently active semester.
    /// </summary>
    Task<List<MentorGroupDto>> GetMentorGroupsAsync(
        Guid mentorId,
        int? semesterId,
        CancellationToken cancellationToken = default);
}
