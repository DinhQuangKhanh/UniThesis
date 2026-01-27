using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.TopicPool
{
    public class TopicRegistrationRejectedEventHandler : IDomainEventHandler<TopicRegistrationRejectedEvent>
    {
        private readonly ILogger<TopicRegistrationRejectedEventHandler> _logger;

        public TopicRegistrationRejectedEventHandler(ILogger<TopicRegistrationRejectedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(TopicRegistrationRejectedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Topic registration rejected: {RegistrationId}, Project: {ProjectId}, Reason: {Reason}",
                @event.RegistrationId, @event.ProjectId, @event.Reason);
            // TODO: Notify group about rejection
            return Task.CompletedTask;
        }
    }
}
