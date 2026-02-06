using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.UserAggregate.Events
{
    /// <summary>
    /// Domain event raised when a role is assigned to a user.
    /// </summary>
    public sealed record UserRoleAssignedEvent(
        Guid UserId,
        string RoleName,
        Guid? AssignedBy
    ) : DomainEventBase;
}
