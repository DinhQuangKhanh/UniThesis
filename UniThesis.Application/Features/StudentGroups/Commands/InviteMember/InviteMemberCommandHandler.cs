using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Notification;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;

namespace UniThesis.Application.Features.StudentGroups.Commands.InviteMember;

public class InviteMemberCommandHandler : ICommandHandler<InviteMemberCommand, int>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public InviteMemberCommandHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        INotificationService notificationService)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<int> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        var inviterId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa được xác thực.");

        var group = await _groupRepository.GetWithInvitationsAsync(request.GroupId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Group), request.GroupId);

        // Find the student by code
        var invitee = await _userRepository.GetByStudentCodeAsync(request.StudentCode, cancellationToken)
            ?? throw new EntityNotFoundException("User", request.StudentCode);

        // Check if invitee is already in an active group this semester
        if (await _groupRepository.IsStudentInActiveGroupAsync(invitee.Id, group.SemesterId, cancellationToken))
            throw new BusinessRuleValidationException("Sinh viên này đã có nhóm hoạt động trong học kỳ này.");

        // Domain logic validates leader, capacity, duplicates
        var invitation = group.InviteMember(inviterId, invitee.Id, request.Message);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsPendingInvitationUniqueViolation(ex))
        {
            throw new BusinessRuleValidationException("Sinh viên này đã có lời mời tham gia nhóm đang chờ xử lý.");
        }

        var inviterDisplayName = _currentUser.FullName ?? _currentUser.Email ?? "Một sinh viên";
        await _notificationService.SendAsync(
            invitee.Id,
            "Bạn nhận được lời mời tham gia nhóm",
            $"{inviterDisplayName} đã mời bạn tham gia nhóm {group.Code.Value}. Vui lòng phản hồi trước khi lời mời hết hạn.",
            NotificationType.Info,
            NotificationCategory.Group,
            "/student/invitations",
            cancellationToken);

        return invitation.Id;
    }

    private static bool IsPendingInvitationUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException?.Message.Contains("IX_GroupInvitations_GroupId_InviteeId_Pending", StringComparison.OrdinalIgnoreCase) == true
            || ex.Message.Contains("IX_GroupInvitations_GroupId_InviteeId_Pending", StringComparison.OrdinalIgnoreCase);
    }
}
