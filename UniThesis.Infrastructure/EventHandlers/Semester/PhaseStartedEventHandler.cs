using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Aggregates.SemesterAggregate.Events;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Enums.Notification;
using UniThesis.Domain.Enums.Semester;
using UniThesis.Persistence.SqlServer.Constants;

namespace UniThesis.Infrastructure.EventHandlers.Semester;

public class PhaseStartedEventHandler : INotificationHandler<PhaseStartedEvent>, IInfrastructureEventHandlerMarker
{
    private readonly ISemesterRepository _semesterRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<PhaseStartedEventHandler> _logger;

    public PhaseStartedEventHandler(
        ISemesterRepository semesterRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<PhaseStartedEventHandler> logger)
    {
        _semesterRepository = semesterRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(PhaseStartedEvent notification, CancellationToken cancellationToken)
    {
        switch (notification.PhaseType)
        {
            case SemesterPhaseType.Registration:
                await HandleRegistrationStarted(notification, cancellationToken);
                break;
            case SemesterPhaseType.Evaluation:
                await HandleEvaluationStarted(notification, cancellationToken);
                break;
            default:
                _logger.LogInformation("No notification configured for started phase type {PhaseType}", notification.PhaseType);
                break;
        }
    }

    private async Task HandleRegistrationStarted(PhaseStartedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Topic Registration phase started for SemesterId={SemesterId}. Initializing system notifications...",
            notification.SemesterId);

        var semester = await _semesterRepository.GetByIdAsync(notification.SemesterId, cancellationToken);
        var semesterCodeStr = semester != null ? semester.Code.Value : notification.SemesterId.ToString();

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
                title: "Thời gian đăng ký đề tài",
                content: $"Thời gian đăng ký đề tài trong học kỳ {semesterCodeStr} đã chính thức bắt đầu, vui lòng truy cập hệ thống để thực hiện các thao tác quản lý và đăng ký hoặc theo dõi tiến độ công việc.",
                type: NotificationType.Info,
                category: NotificationCategory.System,
                targetUrl: "/topic-pool",
                ct: cancellationToken);

            _logger.LogInformation(
                "Dispatched Registration started notifications to {Count} users for SemesterCode={SemesterCode}.",
                targetUserIds.Count, semesterCodeStr);
        }
    }

    private async Task HandleEvaluationStarted(PhaseStartedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Evaluation phase started for SemesterId={SemesterId}. Sending notifications to lecturers...",
            notification.SemesterId);

        var semester = await _semesterRepository.GetByIdAsync(notification.SemesterId, cancellationToken);
        var semesterCodeStr = semester != null ? semester.Code.Value : notification.SemesterId.ToString();

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
                title: "Thời gian thẩm định đề tài đã bắt đầu",
                content: $"Thời gian thẩm định đề tài trong học kỳ {semesterCodeStr} đã chính thức bắt đầu. Quý thầy cô vui lòng truy cập hệ thống để thực hiện thẩm định các đề tài.",
                type: NotificationType.Info,
                category: NotificationCategory.System,
                targetUrl: null,
                ct: cancellationToken);

            _logger.LogInformation(
                "Dispatched Evaluation started notifications to {Count} lecturers for SemesterCode={SemesterCode}.",
                targetUserIds.Count, semesterCodeStr);
        }
        else
        {
            _logger.LogWarning("No lecturers found to notify for Evaluation phase start (SemesterCode={SemesterCode})", semesterCodeStr);
        }
    }
}
