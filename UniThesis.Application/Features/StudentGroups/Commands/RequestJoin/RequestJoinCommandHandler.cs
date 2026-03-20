using UniThesis.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Notification;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;
using UniThesis.Application.Common.Interfaces;

namespace UniThesis.Application.Features.StudentGroups.Commands.RequestJoin;

public class RequestJoinCommandHandler : ICommandHandler<RequestJoinCommand, int>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public RequestJoinCommandHandler(
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

    public async Task<int> Handle(RequestJoinCommand request, CancellationToken cancellationToken)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Người dùng chưa được xác thực.");

        var group = await _groupRepository.GetWithJoinRequestsAsync(request.GroupId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Group), request.GroupId);

        // Check if student is already in an active group this semester
        if (await _groupRepository.IsStudentInActiveGroupAsync(studentId, group.SemesterId, cancellationToken))
            throw new BusinessRuleValidationException("Bạn đã có nhóm hoạt động trong học kỳ này.");

        // A student can only keep one pending join request at a time in the same semester.
        if (await _groupRepository.HasPendingJoinRequestAsync(studentId, group.SemesterId, cancellationToken))
            throw new BusinessRuleValidationException("Bạn đã có một yêu cầu tham gia nhóm đang chờ xử lý trong học kỳ này.");

        // Domain logic validates group status, open for requests, capacity, duplicates
        var joinRequest = group.RequestToJoin(studentId, request.Message);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsPendingJoinRequestUniqueViolation(ex))
        {
            throw new BusinessRuleValidationException("Bạn đã có một yêu cầu tham gia nhóm đang chờ xử lý.");
        }

        if (group.LeaderId.HasValue)
        {
            var studentDisplayName = _currentUser.FullName ?? _currentUser.Email ?? "Một sinh viên";
            await _notificationService.SendAsync(
                group.LeaderId.Value,
                "Có yêu cầu tham gia nhóm mới",
                $"{studentDisplayName} vừa gửi yêu cầu tham gia nhóm {group.Code.Value}. Vui lòng phản hồi trong vòng 1 giờ.",
                NotificationType.Info,
                NotificationCategory.Group,
                $"/student/group-detail",
                cancellationToken);
        }

        return joinRequest.Id;
    }

    private static bool IsPendingJoinRequestUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException?.Message.Contains("IX_GroupJoinRequests_GroupId_StudentId_Pending", StringComparison.OrdinalIgnoreCase) == true
            || ex.Message.Contains("IX_GroupJoinRequests_GroupId_StudentId_Pending", StringComparison.OrdinalIgnoreCase);
    }
}
