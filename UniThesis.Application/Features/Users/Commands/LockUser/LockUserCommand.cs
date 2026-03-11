using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Users.Commands.LockUser;

/// <summary>
/// Command to lock a user account, disabling their access via both database and Firebase.
/// Invalidates user list cache after successful execution.
/// </summary>
public record LockUserCommand(Guid UserId) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["users:list:"];
}
