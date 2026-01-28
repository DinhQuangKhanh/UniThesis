using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.MeetingAggregate.Events;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Meeting
{
    public class MeetingRequestedEventHandler : INotificationHandler<MeetingRequestedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MeetingRequestedEventHandler> _logger;

        public MeetingRequestedEventHandler(INotificationService notificationService, ILogger<MeetingRequestedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(MeetingRequestedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Meeting requested: {MeetingId} by Group {GroupId}", notification.MeetingId, notification.GroupId);
            await Task.CompletedTask;
        }
    }
}
