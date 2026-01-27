using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Enums.Notification;
using UniThesis.Infrastructure.SignalR;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubNotificationService? _hubNotificationService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger, IHubNotificationService? hubNotificationService = null)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
            _hubNotificationService = hubNotificationService;
        }

        public async Task SendAsync(Guid userId, string title, string content, NotificationType type, NotificationCategory category, string? targetUrl = null, CancellationToken ct = default)
        {
            var notification = new NotificationDocument { UserId = userId, Title = title, Content = content, Type = type, Category = category, TargetUrl = targetUrl, CreatedAt = DateTime.UtcNow };
            await _notificationRepository.AddAsync(notification, ct);
            if (_hubNotificationService is not null)
                await _hubNotificationService.SendToUserAsync(userId, "NewNotification", new { notification.Id, notification.Title, notification.Content, notification.Type, notification.CreatedAt });
            _logger.LogInformation("Notification sent to user {UserId}: {Title}", userId, title);
        }

        public async Task SendToMultipleAsync(IEnumerable<Guid> userIds, string title, string content, NotificationType type, NotificationCategory category, string? targetUrl = null, CancellationToken ct = default)
        {
            var notifications = userIds.Select(userId => new NotificationDocument { UserId = userId, Title = title, Content = content, Type = type, Category = category, TargetUrl = targetUrl, CreatedAt = DateTime.UtcNow });
            await _notificationRepository.AddManyAsync(notifications, ct);
            if (_hubNotificationService is not null)
                foreach (var userId in userIds)
                    await _hubNotificationService.SendToUserAsync(userId, "NewNotification", new { Title = title, Content = content, Type = type });
        }

        public async Task<IEnumerable<NotificationDocument>> GetUserNotificationsAsync(Guid userId, int limit = 50, CancellationToken ct = default) => await _notificationRepository.GetByUserIdAsync(userId, limit, ct);
        public async Task<IEnumerable<NotificationDocument>> GetUnreadNotificationsAsync(Guid userId, CancellationToken ct = default) => await _notificationRepository.GetUnreadByUserIdAsync(userId, ct);
        public async Task<long> GetUnreadCountAsync(Guid userId, CancellationToken ct = default) => await _notificationRepository.GetUnreadCountAsync(userId, ct);
        public async Task MarkAsReadAsync(Guid notificationId, CancellationToken ct = default) => await _notificationRepository.MarkAsReadAsync(notificationId, ct);
        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default) => await _notificationRepository.MarkAllAsReadAsync(userId, ct);

        public async Task NotifyProjectSubmittedAsync(Guid projectId, string projectName, Guid submittedBy, CancellationToken ct = default)
            => await SendAsync(submittedBy, "Đề tài đã được nộp", $"Đề tài '{projectName}' đã được nộp thành công.", NotificationType.Info, NotificationCategory.Project, $"/projects/{projectId}", ct);

        public async Task NotifyProjectApprovedAsync(Guid projectId, string projectName, Guid groupLeaderId, CancellationToken ct = default)
            => await SendAsync(groupLeaderId, "Đề tài được duyệt", $"Đề tài '{projectName}' đã được duyệt.", NotificationType.Success, NotificationCategory.Project, $"/projects/{projectId}", ct);

        public async Task NotifyEvaluationAssignedAsync(Guid evaluatorId, Guid projectId, string projectName, CancellationToken ct = default)
            => await SendAsync(evaluatorId, "Phân công thẩm định", $"Bạn được phân công thẩm định đề tài '{projectName}'.", NotificationType.Info, NotificationCategory.Evaluation, $"/evaluations/{projectId}", ct);

        public async Task SendTopicExpirationWarningAsync(Project topic, CancellationToken ct = default)
            => await SendAsync((Guid) topic.SubmittedBy, "Đề tài sắp hết hạn", $"Đề tài '{topic.NameVi}' sẽ hết hạn.", NotificationType.Warning, NotificationCategory.Deadline, $"/topics/{topic.Id}", ct);

        public async Task NotifyMeetingApprovedAsync(Guid requesterId, string meetingTitle, DateTime scheduledTime, CancellationToken ct = default)
            => await SendAsync(requesterId, "Lịch hẹn được duyệt", $"Lịch hẹn '{meetingTitle}' được duyệt vào {scheduledTime:dd/MM/yyyy HH:mm}.", NotificationType.Success, NotificationCategory.Meeting, "/meetings", ct);

        public async Task NotifyDefenseScheduledAsync(IEnumerable<Guid> memberIds, string projectName, DateTime defenseTime, string location, CancellationToken ct = default)
            => await SendToMultipleAsync(memberIds, "Lịch bảo vệ đồ án", $"Đề tài '{projectName}' bảo vệ vào {defenseTime:dd/MM/yyyy HH:mm} tại {location}.", NotificationType.Info, NotificationCategory.Defense, "/defense", ct);
    }
}
