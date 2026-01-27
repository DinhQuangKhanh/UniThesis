using UniThesis.Domain.Aggregates.TopicPoolAggregate.Entities;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Specifications.TopicRegistrations;

/// <summary>
/// Specification to check if a group has a pending registration for a project.
/// Used to prevent duplicate registrations.
/// </summary>
public class GroupPendingRegistrationForProjectSpec : BaseSpecification<TopicRegistration>
{
    /// <summary>
    /// Checks if a group has a pending registration for a specific project.
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <param name="projectId">Project ID</param>
    public GroupPendingRegistrationForProjectSpec(Guid groupId, Guid projectId)
        : base(r => r.GroupId == groupId &&
                    r.ProjectId == projectId &&
                    r.Status == TopicRegistrationStatus.Pending)
    {
    }
}
