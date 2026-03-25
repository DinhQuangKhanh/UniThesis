using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.SemesterAggregate.Events;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Enums.Notification;
using UniThesis.Domain.Enums.Semester;
using UniThesis.Persistence.SqlServer.Constants;

namespace UniThesis.Infrastructure.EventHandlers.Semester;

public class PhaseUpcomingEventHandler : INotificationHandler<PhaseUpcomingEvent>, IInfrastructureEventHandlerMarker
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<PhaseUpcomingEventHandler> _logger;

    public PhaseUpcomingEventHandler(
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<PhaseUpcomingEventHandler> logger)
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(PhaseUpcomingEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Phase upcoming event triggered: SemesterId={SemesterId}, PhaseId={PhaseId}, PhaseType={PhaseType}",
            notification.SemesterId,
            notification.PhaseId,
            notification.PhaseType);

        switch (notification.PhaseType)
        {
            case SemesterPhaseType.Registration:
                await HandleRegistrationUpcoming(notification, cancellationToken);
                break;
            case SemesterPhaseType.Evaluation:
                await HandleEvaluationUpcoming(notification, cancellationToken);
                break;
            default:
                _logger.LogInformation("No notification configured for upcoming phase type {PhaseType}", notification.PhaseType);
                break;
        }
    }

    private async Task HandleRegistrationUpcoming(PhaseUpcomingEvent notification, CancellationToken cancellationToken)
    {
        // Get all students
        var students = await _userRepository.GetByRoleAsync(RoleNames.Student, cancellationToken);
        
        // Get lecturers
        var mentors = await _userRepository.GetByRoleAsync(RoleNames.Mentor, cancellationToken);
        var evaluators = await _userRepository.GetByRoleAsync(RoleNames.Evaluator, cancellationToken);
        var departmentHeads = await _userRepository.GetByRoleAsync(RoleNames.DepartmentHead, cancellationToken);

        var targetUserIds = students.Select(u => u.Id)
            .Union(mentors.Select(u => u.Id))
            .Union(evaluators.Select(u => u.Id))
            .Union(departmentHeads.Select(u => u.Id))
            .Distinct()
            .ToList();

        if (targetUserIds.Count > 0)
        {
            await _notificationService.SendToMultipleAsync(
                userIds: targetUserIds,
                title: "Sắp tới hạn đăng ký đề tài/đồ án",
                content: "Thời gian đăng ký đề tài cho học kỳ sắp diễn ra (còn 3 ngày). Quý thầy cô và sinh viên vui lòng chuẩn bị.",
                type: NotificationType.Warning,
                category: NotificationCategory.System,
                targetUrl: null,
                ct: cancellationToken);

            _logger.LogInformation(
                "Dispatched Registration upcoming notifications to {Count} users for SemesterId={SemesterId}.",
                targetUserIds.Count, notification.SemesterId);
        }
    }

    private async Task HandleEvaluationUpcoming(PhaseUpcomingEvent notification, CancellationToken cancellationToken)
    {
        // Only notify lecturers (Mentor, Evaluator, DepartmentHead)
        var mentors = await _userRepository.GetByRoleAsync(RoleNames.Mentor, cancellationToken);
        var evaluators = await _userRepository.GetByRoleAsync(RoleNames.Evaluator, cancellationToken);
        var departmentHeads = await _userRepository.GetByRoleAsync(RoleNames.DepartmentHead, cancellationToken);

        var targetUserIds = mentors.Select(u => u.Id)
            .Union(evaluators.Select(u => u.Id))
            .Union(departmentHeads.Select(u => u.Id))
            .Distinct()
            .ToList();

        if (targetUserIds.Count > 0)
        {
            await _notificationService.SendToMultipleAsync(
                userIds: targetUserIds,
                title: "Sắp tới thời gian thẩm định đề tài",
                content: "Thời gian thẩm định đề tài cho học kỳ sắp diễn ra (còn 3 ngày). Quý thầy cô vui lòng chuẩn bị thẩm định các đề tài đã đăng ký.",
                type: NotificationType.Warning,
                category: NotificationCategory.System,
                targetUrl: null,
                ct: cancellationToken);

            _logger.LogInformation(
                "Dispatched Evaluation upcoming notifications to {Count} lecturers for SemesterId={SemesterId}.",
                targetUserIds.Count, notification.SemesterId);
        }
        else
        {
            _logger.LogWarning("No lecturers found to notify for upcoming Evaluation phase in SemesterId={SemesterId}", notification.SemesterId);
        }
    }
}
