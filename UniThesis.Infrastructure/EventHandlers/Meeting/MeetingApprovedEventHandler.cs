using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.MeetingAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Meeting
{
    public class MeetingApprovedEventHandler : IDomainEventHandler<MeetingApprovedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MeetingApprovedEventHandler> _logger;

        public MeetingApprovedEventHandler(INotificationService notificationService, ILogger<MeetingApprovedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(MeetingApprovedEvent @event, CancellationToken ct = default)
        {
            _logger.LogInformation("Meeting approved: {MeetingId}", @event.MeetingId);
            await Task.CompletedTask;
        }
    }
}
