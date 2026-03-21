using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.StudentGroups.Commands.RequestJoin;

[ActionLog("Request Join", "StudentGroup")]
public record RequestJoinCommand(Guid GroupId, string? Message) : ICacheInvalidatingCommand<int>
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["student-groups:"];
}
