using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.TopicPool
{
    public class TopicRegistrationRequestedEventHandler : IDomainEventHandler<TopicRegistrationRequestedEvent>
    {
        private readonly ILogger<TopicRegistrationRequestedEventHandler> _logger;

        public TopicRegistrationRequestedEventHandler(ILogger<TopicRegistrationRequestedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(TopicRegistrationRequestedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Topic registration requested: {RegistrationId}, Project: {ProjectId}, Group: {GroupId}",
                @event.RegistrationId, @event.ProjectId, @event.GroupId);
            // TODO: Notify mentor about pending registration
            return Task.CompletedTask;
        }
    }
}
