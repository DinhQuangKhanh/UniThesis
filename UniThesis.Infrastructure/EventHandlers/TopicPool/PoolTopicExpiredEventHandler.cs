using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.TopicPool
{
    public class PoolTopicExpiredEventHandler : IDomainEventHandler<PoolTopicExpiredEvent>
    {
        private readonly ILogger<PoolTopicExpiredEventHandler> _logger;

        public PoolTopicExpiredEventHandler(ILogger<PoolTopicExpiredEventHandler> logger) => _logger = logger;

        public Task HandleAsync(PoolTopicExpiredEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Pool topic expired: {ProjectId}, Pool: {PoolId}, Created: {CreatedSemester}, Expired: {ExpiredSemester}",
                @event.ProjectId, @event.TopicPoolId, @event.CreatedSemesterId, @event.ExpiredAtSemesterId);
            // TODO: Notify mentor about expired topic
            return Task.CompletedTask;
        }
    }
}
