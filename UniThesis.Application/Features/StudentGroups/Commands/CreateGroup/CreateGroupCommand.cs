using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.StudentGroups.Commands.CreateGroup;

public record CreateGroupCommand(string? Name) : ICommand<Guid>;
