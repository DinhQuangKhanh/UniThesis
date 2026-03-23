using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.StudentGroups.Commands.RequestJoin;

public record RequestJoinCommand(Guid GroupId, string? Message) : ICommand<int>;
