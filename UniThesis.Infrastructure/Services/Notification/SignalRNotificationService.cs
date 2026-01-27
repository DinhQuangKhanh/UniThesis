using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using UniThesis.Infrastructure.SignalR;

namespace UniThesis.Infrastructure.Services.Notification
{
    /// <summary>
    /// SignalR-based real-time notification service.
    /// </summary>
    public class SignalRNotificationService : IHubNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<SignalRNotificationService> _logger;

        public SignalRNotificationService(
            IHubContext<NotificationHub> hubContext,
            ILogger<SignalRNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Sends a notification to a specific user.
        /// </summary>
        public async Task SendToUserAsync(Guid userId, string method, object message, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients
                    .Group($"user_{userId}")
                    .SendAsync(method, message, cancellationToken);

                _logger.LogDebug("SignalR notification sent to user {UserId}: {Method}", userId, method);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SignalR notification to user {UserId}", userId);
            }
        }

        /// <summary>
        /// Sends a notification to multiple users.
        /// </summary>
        public async Task SendToUsersAsync(IEnumerable<Guid> userIds, string method, object message, CancellationToken cancellationToken = default)
        {
            try
            {
                var groups = userIds.Select(id => $"user_{id}").ToList();
                await _hubContext.Clients
                    .Groups(groups)
                    .SendAsync(method, message, cancellationToken);

                _logger.LogDebug("SignalR notification sent to {Count} users: {Method}", groups.Count, method);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SignalR notification to multiple users");
            }
        }

        /// <summary>
        /// Sends a notification to a specific group.
        /// </summary>
        public async Task SendToGroupAsync(string groupName, string method, object message, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients
                    .Group(groupName)
                    .SendAsync(method, message, cancellationToken);

                _logger.LogDebug("SignalR notification sent to group {Group}: {Method}", groupName, method);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SignalR notification to group {Group}", groupName);
            }
        }

        /// <summary>
        /// Sends a notification to all connected clients.
        /// </summary>
        public async Task SendToAllAsync(string method, object message, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync(method, message, cancellationToken);

                _logger.LogDebug("SignalR notification broadcast: {Method}", method);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting SignalR notification");
            }
        }

        /// <summary>
        /// Sends a notification to all clients except specified connections.
        /// </summary>
        public async Task SendToAllExceptAsync(IEnumerable<string> excludedConnectionIds, string method, object message, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients
                    .AllExcept(excludedConnectionIds.ToList())
                    .SendAsync(method, message, cancellationToken);

                _logger.LogDebug("SignalR notification sent to all except {Count} connections: {Method}",
                    excludedConnectionIds.Count(), method);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SignalR notification to all except");
            }
        }

        /// <summary>
        /// Sends a new notification alert to a user.
        /// </summary>
        public async Task SendNotificationAlertAsync(Guid userId, NotificationAlert alert, CancellationToken cancellationToken = default)
        {
            await SendToUserAsync(userId, "ReceiveNotification", alert, cancellationToken);
        }

        /// <summary>
        /// Sends an unread count update to a user.
        /// </summary>
        public async Task SendUnreadCountUpdateAsync(Guid userId, long unreadCount, CancellationToken cancellationToken = default)
        {
            await SendToUserAsync(userId, "UnreadCountUpdated", new { count = unreadCount }, cancellationToken);
        }

        /// <summary>
        /// Sends a project status update to all group members.
        /// </summary>
        public async Task SendProjectStatusUpdateAsync(Guid groupId, ProjectStatusUpdate update, CancellationToken cancellationToken = default)
        {
            await SendToGroupAsync($"group_{groupId}", "ProjectStatusUpdated", update, cancellationToken);
        }

        /// <summary>
        /// Sends a meeting reminder to participants.
        /// </summary>
        public async Task SendMeetingReminderAsync(IEnumerable<Guid> participantIds, MeetingReminder reminder, CancellationToken cancellationToken = default)
        {
            await SendToUsersAsync(participantIds, "MeetingReminder", reminder, cancellationToken);
        }

        /// <summary>
        /// Sends a defense schedule notification to council members and group.
        /// </summary>
        public async Task SendDefenseNotificationAsync(IEnumerable<Guid> recipientIds, DefenseNotification notification, CancellationToken cancellationToken = default)
        {
            await SendToUsersAsync(recipientIds, "DefenseScheduled", notification, cancellationToken);
        }
    }
}
