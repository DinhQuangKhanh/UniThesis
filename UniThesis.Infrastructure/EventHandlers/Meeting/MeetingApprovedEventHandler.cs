using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.MeetingAggregate.Events;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Meeting
{
    public class MeetingApprovedEventHandler : INotificationHandler<MeetingApprovedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MeetingApprovedEventHandler> _logger;

        public MeetingApprovedEventHandler(INotificationService notificationService, ILogger<MeetingApprovedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(MeetingApprovedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Meeting approved: {MeetingId}", notification.MeetingId);
            await Task.CompletedTask;
        }
    }
}
