using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Semesters.Commands.CloseSemester;

/// <summary>
/// Command to close an active semester.
/// </summary>
public record CloseSemesterCommand(int SemesterId) : ICommand;
