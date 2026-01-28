using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.DefenseAggregate.Events;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Defense
{
    public class DefenseScheduledEventHandler : INotificationHandler<DefenseScheduledEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<DefenseScheduledEventHandler> _logger;

        public DefenseScheduledEventHandler(INotificationService notificationService, ILogger<DefenseScheduledEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(DefenseScheduledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Defense scheduled: {DefenseId} for group {GroupId}", notification.DefenseId, notification.GroupId);
            await Task.CompletedTask;
        }
    }
}
