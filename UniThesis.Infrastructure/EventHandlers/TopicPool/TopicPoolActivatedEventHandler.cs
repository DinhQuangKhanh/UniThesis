using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.TopicPool
{
    public class TopicPoolActivatedEventHandler : IDomainEventHandler<TopicPoolActivatedEvent>
    {
        private readonly ILogger<TopicPoolActivatedEventHandler> _logger;

        public TopicPoolActivatedEventHandler(ILogger<TopicPoolActivatedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(TopicPoolActivatedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Topic pool activated: {PoolId}", @event.TopicPoolId);
            return Task.CompletedTask;
        }
    }
}
