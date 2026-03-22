using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;

namespace UniThesis.Infrastructure.EventHandlers.Group
{
    public class MemberAddedEventHandler : INotificationHandler<MemberAddedEvent>
    {
        private readonly ILogger<MemberAddedEventHandler> _logger;

        public MemberAddedEventHandler(ILogger<MemberAddedEventHandler> logger) => _logger = logger;

        public Task Handle(MemberAddedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Member added to group: {StudentId} -> {GroupId}", notification.StudentId, notification.GroupId);
            return Task.CompletedTask;
        }
    }
}
