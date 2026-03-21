using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.StudentGroups.Commands.InviteMember;

[ActionLog("Invite Member", "StudentGroup")]
public record InviteMemberCommand(Guid GroupId, string StudentCode, string? Message) : ICacheInvalidatingCommand<int>
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["student-groups:"];
}
