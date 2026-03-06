using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Semesters.Commands.ActivateSemester;

/// <summary>
/// Command to activate an upcoming semester.
/// </summary>
public record ActivateSemesterCommand(int SemesterId) : ICommand;
