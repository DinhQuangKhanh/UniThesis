using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Notification;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;

namespace UniThesis.Application.Features.StudentGroups.Commands.RespondInvitation;

public class RespondInvitationCommandHandler : ICommandHandler<RespondInvitationCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public RespondInvitationCommandHandler(
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        INotificationService notificationService)
    {
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(RespondInvitationCommand request, CancellationToken cancellationToken)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa được xác thực.");

        var group = await _groupRepository.GetWithInvitationsAsync(request.GroupId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Group), request.GroupId);

        var invitation = group.Invitations.FirstOrDefault(i => i.Id == request.InvitationId)
            ?? throw new EntityNotFoundException("GroupInvitation", request.InvitationId);

        var studentDisplayName = _currentUser.FullName ?? _currentUser.Email ?? "Một sinh viên";

        if (request.Accept)
        {
            // Check if student is already in another active group
            if (await _groupRepository.IsStudentInActiveGroupAsync(studentId, group.SemesterId, cancellationToken))
                throw new BusinessRuleValidationException("Bạn đã có nhóm hoạt động trong học kỳ này.");

            group.AcceptInvitation(request.InvitationId, studentId);

            // Notify inviter that invitation was accepted
            await _notificationService.SendAsync(
                invitation.InviterId,
                "Lời mời được chấp nhận",
                $"{studentDisplayName} đã chấp nhận lời mời tham gia nhóm {group.Code.Value}.",
                NotificationType.Success,
                NotificationCategory.Group,
                $"/student/group-detail",
                cancellationToken);
        }
        else
        {
            group.RejectInvitation(request.InvitationId, studentId);

            // Notify inviter that invitation was rejected
            await _notificationService.SendAsync(
                invitation.InviterId,
                "Lời mời bị từ chối",
                $"{studentDisplayName} đã từ chối lời mời tham gia nhóm {group.Code.Value}.",
                NotificationType.Info,
                NotificationCategory.Group,
                $"/student/group-detail",
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
