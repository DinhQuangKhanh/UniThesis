using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Enums.Notification;
using UniThesis.Persistence.MongoDB.Documents;

namespace UniThesis.Infrastructure.Services.Notification
{
    public interface INotificationService
    {
        Task SendAsync(Guid userId, string title, string content, NotificationType type, NotificationCategory category, string? targetUrl = null, CancellationToken ct = default);
        Task SendToMultipleAsync(IEnumerable<Guid> userIds, string title, string content, NotificationType type, NotificationCategory category, string? targetUrl = null, CancellationToken ct = default);
        Task<IEnumerable<NotificationDocument>> GetUserNotificationsAsync(Guid userId, int limit = 50, CancellationToken ct = default);
        Task<IEnumerable<NotificationDocument>> GetUnreadNotificationsAsync(Guid userId, CancellationToken ct = default);
        Task<long> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
        Task MarkAsReadAsync(Guid notificationId, CancellationToken ct = default);
        Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
        Task NotifyProjectSubmittedAsync(Guid projectId, string projectName, Guid submittedBy, CancellationToken ct = default);
        Task NotifyProjectApprovedAsync(Guid projectId, string projectName, Guid groupLeaderId, CancellationToken ct = default);
        Task NotifyEvaluationAssignedAsync(Guid evaluatorId, Guid projectId, string projectName, CancellationToken ct = default);
        Task SendTopicExpirationWarningAsync(Project topic, CancellationToken ct = default);
        Task NotifyMeetingApprovedAsync(Guid requesterId, string meetingTitle, DateTime scheduledTime, CancellationToken ct = default);
        Task NotifyDefenseScheduledAsync(IEnumerable<Guid> memberIds, string projectName, DateTime defenseTime, string location, CancellationToken ct = default);
    }
}
