using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Semesters.Commands.ActivateSemester;

/// <summary>
/// Command to activate an upcoming semester.
/// </summary>
[ActionLog("Activate Semester", "Semester")]
public record ActivateSemesterCommand(int SemesterId) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate => ["admin:dashboard"];
}
