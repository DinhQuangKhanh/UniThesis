using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Semesters.Commands.CreateSemester;

/// <summary>
/// Command to create a new semester with optional phases.
/// </summary>
public record CreateSemesterCommand(
    string Name,
    string Code,
    DateTime StartDate,
    DateTime EndDate,
    int AcademicYearStart,
    string? Description,
    List<CreateSemesterPhaseDto> Phases
) : ICacheInvalidatingCommand<int>
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate => ["admin:dashboard", "evaluator:filter-options"];
}

/// <summary>
/// DTO for creating a phase as part of semester creation.
/// </summary>
public record CreateSemesterPhaseDto(
    string Name,
    string Type,
    DateTime StartDate,
    DateTime EndDate
);
