using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Semesters.Commands.DeleteSemester;

/// <summary>
/// Command to delete an existing semester.
/// </summary>
[ActionLog("Delete Semester", "Semester")]
public record DeleteSemesterCommand(int Id) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["semesters:", "admin:dashboard", "evaluator:filter-options"];
}
