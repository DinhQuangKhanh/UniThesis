using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.StudentGroups.Commands.CreateGroup;

[ActionLog("Create Group", "StudentGroup")]
public record CreateGroupCommand(string? Name) : ICacheInvalidatingCommand<Guid>
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["student-groups:"];
}
