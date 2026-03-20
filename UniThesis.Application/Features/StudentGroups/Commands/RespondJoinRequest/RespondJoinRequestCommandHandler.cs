using MediatR;
using System.Linq;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Notification;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;

namespace UniThesis.Application.Features.StudentGroups.Commands.RespondJoinRequest;

public class RespondJoinRequestCommandHandler : ICommandHandler<RespondJoinRequestCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public RespondJoinRequestCommandHandler(
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

    public async Task<Unit> Handle(RespondJoinRequestCommand request, CancellationToken cancellationToken)
    {
        var leaderId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa được xác thực.");

        var group = await _groupRepository.GetWithJoinRequestsAsync(request.GroupId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Group), request.GroupId);

        if (request.Approve)
        {
            var joinRequest = group.JoinRequests.FirstOrDefault(r => r.Id == request.RequestId)
                ?? throw new EntityNotFoundException("GroupJoinRequest", request.RequestId);

            if (await _groupRepository.IsStudentInActiveGroupAsync(joinRequest.StudentId, group.SemesterId, cancellationToken))
                throw new BusinessRuleValidationException("Sinh viên đã có nhóm hoạt động trong học kỳ này.");

            group.ApproveJoinRequest(request.RequestId, leaderId);

            // Notify student that join request was approved
            await _notificationService.SendAsync(
                joinRequest.StudentId,
                "Yêu cầu được phê duyệt",
                $"Yêu cầu tham gia nhóm {group.Code.Value} của bạn đã được phê duyệt.",
                NotificationType.Success,
                NotificationCategory.Group,
                $"/student/group-detail",
                cancellationToken);
        }
        else
        {
            var joinRequest = group.JoinRequests.FirstOrDefault(r => r.Id == request.RequestId)
                ?? throw new EntityNotFoundException("GroupJoinRequest", request.RequestId);

            group.RejectJoinRequest(request.RequestId, leaderId);

            // Notify student that join request was rejected
            await _notificationService.SendAsync(
                joinRequest.StudentId,
                "Yêu cầu bị từ chối",
                $"Yêu cầu tham gia nhóm {group.Code.Value} của bạn đã bị từ chối.",
                NotificationType.Info,
                NotificationCategory.Group,
                $"/student/group-detail",
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
