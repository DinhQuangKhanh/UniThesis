using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Domain.Enums.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Group
{
    public class JoinRequestRejectedEventHandler : INotificationHandler<JoinRequestRejectedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<JoinRequestRejectedEventHandler> _logger;

        public JoinRequestRejectedEventHandler(
            INotificationService notificationService,
            ILogger<JoinRequestRejectedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(JoinRequestRejectedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Join request rejected: GroupId={GroupId}, StudentId={StudentId}",
                notification.GroupId, notification.StudentId);

            try
            {
                await _notificationService.SendAsync(
                    notification.StudentId,
                    "Yêu cầu bị từ chối",
                    $"Yêu cầu tham gia nhóm {notification.GroupCode} của bạn đã bị từ chối.",
                    NotificationType.Info,
                    NotificationCategory.Group,
                    "/student/group-detail",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling JoinRequestRejectedEvent for group {GroupId}", notification.GroupId);
            }
        }
    }
}
