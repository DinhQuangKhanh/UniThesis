using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Semesters.Commands.UpdateSemester;

/// <summary>
/// Command to update an existing semester's details and phase dates.
/// Only allowed when the semester status is Upcoming.
/// </summary>
public record UpdateSemesterCommand(
    int Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string? Description,
    List<UpdatePhaseDto>? Phases
) : ICommand;

/// <summary>
/// DTO for updating phase dates within a semester.
/// </summary>
public record UpdatePhaseDto(int Id, DateTime StartDate, DateTime EndDate);
