using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.DefenseAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Defense
{
    public class DefenseScheduledEventHandler : IDomainEventHandler<DefenseScheduledEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<DefenseScheduledEventHandler> _logger;

        public DefenseScheduledEventHandler(INotificationService notificationService, ILogger<DefenseScheduledEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(DefenseScheduledEvent @event, CancellationToken ct = default)
        {
            _logger.LogInformation("Defense scheduled: {DefenseId} for group {GroupId}", @event.DefenseId, @event.GroupId);
            await Task.CompletedTask;
        }
    }
}
