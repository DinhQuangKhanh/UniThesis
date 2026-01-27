using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Group
{
    public class GroupCreatedEventHandler : IDomainEventHandler<GroupCreatedEvent>
    {
        private readonly ILogger<GroupCreatedEventHandler> _logger;

        public GroupCreatedEventHandler(ILogger<GroupCreatedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(GroupCreatedEvent @event, CancellationToken ct = default)
        {
            _logger.LogInformation("Group created: {GroupId}, Code: {GroupCode}", @event.GroupId, @event.GroupCode);
            return Task.CompletedTask;
        }
    }
}
