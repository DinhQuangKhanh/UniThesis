using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Users.Commands.UnlockUser;

/// <summary>
/// Command to unlock a previously locked user account, re-enabling their access.
/// Invalidates user list cache after successful execution.
/// </summary>
[ActionLog("Unlock User", "User")]
public record UnlockUserCommand(Guid UserId) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["users:list:"];
}
