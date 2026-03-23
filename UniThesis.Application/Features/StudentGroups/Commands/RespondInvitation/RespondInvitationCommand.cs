using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.StudentGroups.Commands.RespondInvitation;

[ActionLog("Respond Invitation", "StudentGroup")]
public record RespondInvitationCommand(Guid GroupId, int InvitationId, bool Accept) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["student-groups:"];
}
