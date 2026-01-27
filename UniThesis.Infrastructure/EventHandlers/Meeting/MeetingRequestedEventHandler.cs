using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.MeetingAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Meeting
{
    public class MeetingRequestedEventHandler : IDomainEventHandler<MeetingRequestedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MeetingRequestedEventHandler> _logger;

        public MeetingRequestedEventHandler(INotificationService notificationService, ILogger<MeetingRequestedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(MeetingRequestedEvent @event, CancellationToken ct = default)
        {
            _logger.LogInformation("Meeting requested: {MeetingId} by Group {GroupId}", @event.MeetingId, @event.GroupId);
            await Task.CompletedTask;
        }
    }
}
