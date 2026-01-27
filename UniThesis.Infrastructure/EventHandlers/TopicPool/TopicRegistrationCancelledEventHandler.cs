using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.TopicPool
{
    public class TopicRegistrationCancelledEventHandler : IDomainEventHandler<TopicRegistrationCancelledEvent>
    {
        private readonly ILogger<TopicRegistrationCancelledEventHandler> _logger;

        public TopicRegistrationCancelledEventHandler(ILogger<TopicRegistrationCancelledEventHandler> logger) => _logger = logger;

        public Task HandleAsync(TopicRegistrationCancelledEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Topic registration cancelled: {RegistrationId}, Project: {ProjectId}, Reason: {Reason}",
                @event.RegistrationId, @event.ProjectId, @event.Reason);
            return Task.CompletedTask;
        }
    }
}
