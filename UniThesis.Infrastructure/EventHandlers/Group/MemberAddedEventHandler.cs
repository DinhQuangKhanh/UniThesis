using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Group
{
    public class MemberAddedEventHandler : IDomainEventHandler<MemberAddedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MemberAddedEventHandler> _logger;

        public MemberAddedEventHandler(INotificationService notificationService, ILogger<MemberAddedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(MemberAddedEvent @event, CancellationToken ct = default)
        {
            _logger.LogInformation("Member added to group: {StudentId} -> {GroupId}", @event.StudentId, @event.GroupId);
            await Task.CompletedTask;
        }
    }
}
