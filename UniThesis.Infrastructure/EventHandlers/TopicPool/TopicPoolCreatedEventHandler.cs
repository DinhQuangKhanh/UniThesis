using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.TopicPool
{
    public class TopicPoolCreatedEventHandler : IDomainEventHandler<TopicPoolCreatedEvent>
    {
        private readonly ILogger<TopicPoolCreatedEventHandler> _logger;

        public TopicPoolCreatedEventHandler(ILogger<TopicPoolCreatedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(TopicPoolCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Topic pool created: {PoolId}, Code: {Code}, MajorId: {MajorId}",
                @event.TopicPoolId, @event.Code, @event.MajorId);
            return Task.CompletedTask;
        }
    }
}
