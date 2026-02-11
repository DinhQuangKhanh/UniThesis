using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Group
{
    public class MemberAddedEventHandler : INotificationHandler<MemberAddedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MemberAddedEventHandler> _logger;

        public MemberAddedEventHandler(INotificationService notificationService, ILogger<MemberAddedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task Handle(MemberAddedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Member added to group: {StudentId} -> {GroupId}", notification.StudentId, notification.GroupId);
            return Task.CompletedTask;
        }
    }
}
