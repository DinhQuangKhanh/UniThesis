using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Departments.Commands.SetDepartmentHead;

/// <summary>
/// Command for Admin to assign a lecturer as Head of Department (CNBM).
/// This will remove the DepartmentHead role from the previous head (if any)
/// and assign it to the new user.
/// </summary>
public record SetDepartmentHeadCommand(
    int DepartmentId,
    Guid UserId
) : ICommand;
