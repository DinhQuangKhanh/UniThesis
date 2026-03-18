using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Semesters.Commands.DeleteSemester;

/// <summary>
/// Command to delete an existing semester.
/// </summary>
public record DeleteSemesterCommand(int Id) : ICommand;
