using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.StudentGroups.Commands.RespondInvitation;

public record RespondInvitationCommand(Guid GroupId, int InvitationId, bool Accept) : ICommand;
