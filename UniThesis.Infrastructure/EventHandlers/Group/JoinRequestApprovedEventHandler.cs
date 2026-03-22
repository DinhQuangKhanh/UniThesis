using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Domain.Enums.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Group
{
    public class JoinRequestApprovedEventHandler : INotificationHandler<JoinRequestApprovedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<JoinRequestApprovedEventHandler> _logger;

        public JoinRequestApprovedEventHandler(
            INotificationService notificationService,
            ILogger<JoinRequestApprovedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(JoinRequestApprovedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Join request approved: GroupId={GroupId}, StudentId={StudentId}",
                notification.GroupId, notification.StudentId);

            try
            {
                await _notificationService.SendAsync(
                    notification.StudentId,
                    "Yêu cầu được phê duyệt",
                    $"Yêu cầu tham gia nhóm {notification.GroupCode} của bạn đã được phê duyệt.",
                    NotificationType.Success,
                    NotificationCategory.Group,
                    "/student/group-detail",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling JoinRequestApprovedEvent for group {GroupId}", notification.GroupId);
            }
        }
    }
}
