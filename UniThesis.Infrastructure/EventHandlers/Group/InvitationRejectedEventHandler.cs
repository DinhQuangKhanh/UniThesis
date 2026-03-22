using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Enums.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Group
{
    public class InvitationRejectedEventHandler : INotificationHandler<InvitationRejectedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<InvitationRejectedEventHandler> _logger;

        public InvitationRejectedEventHandler(
            INotificationService notificationService,
            IUserRepository userRepository,
            ILogger<InvitationRejectedEventHandler> logger)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(InvitationRejectedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Invitation rejected: GroupId={GroupId}, InviteeId={InviteeId}",
                notification.GroupId, notification.InviteeId);

            try
            {
                var invitee = await _userRepository.GetByIdAsync(notification.InviteeId, cancellationToken);
                var studentName = invitee?.FullName ?? "Một sinh viên";

                await _notificationService.SendAsync(
                    notification.InviterId,
                    "Lời mời bị từ chối",
                    $"{studentName} đã từ chối lời mời tham gia nhóm {notification.GroupCode}.",
                    NotificationType.Info,
                    NotificationCategory.Group,
                    "/student/group-detail",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling InvitationRejectedEvent for group {GroupId}", notification.GroupId);
            }
        }
    }
}
