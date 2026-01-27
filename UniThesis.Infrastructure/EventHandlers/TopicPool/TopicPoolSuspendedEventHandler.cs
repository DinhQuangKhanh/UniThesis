using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.TopicPool
{
    public class TopicPoolSuspendedEventHandler : IDomainEventHandler<TopicPoolSuspendedEvent>
    {
        private readonly ILogger<TopicPoolSuspendedEventHandler> _logger;

        public TopicPoolSuspendedEventHandler(ILogger<TopicPoolSuspendedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(TopicPoolSuspendedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Topic pool suspended: {PoolId}", @event.TopicPoolId);
            return Task.CompletedTask;
        }
    }
}
