using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.StudentGroups.Commands.RespondJoinRequest;

[ActionLog("Respond Join Request", "StudentGroup")]
public record RespondJoinRequestCommand(Guid GroupId, int RequestId, bool Approve) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["student-groups:"];
}
