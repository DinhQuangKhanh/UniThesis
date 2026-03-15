using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Semesters.Commands.CloseSemester;

/// <summary>
/// Command to close an active semester.
/// </summary>
[ActionLog("Close Semester", "Semester")]
public record CloseSemesterCommand(int SemesterId) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate => ["admin:dashboard"];
}
