using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.StudentGroups.Commands.InviteMember;

public record InviteMemberCommand(Guid GroupId, string StudentCode, string? Message) : ICommand<int>;
