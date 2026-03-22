using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.StudentGroups.Commands.RespondJoinRequest;

public record RespondJoinRequestCommand(Guid GroupId, int RequestId, bool Approve) : ICommand;
