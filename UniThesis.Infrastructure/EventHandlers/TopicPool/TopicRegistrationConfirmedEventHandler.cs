using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.TopicPool
{
    public class TopicRegistrationConfirmedEventHandler : IDomainEventHandler<TopicRegistrationConfirmedEvent>
    {
        private readonly ILogger<TopicRegistrationConfirmedEventHandler> _logger;

        public TopicRegistrationConfirmedEventHandler(ILogger<TopicRegistrationConfirmedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(TopicRegistrationConfirmedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Topic registration confirmed: {RegistrationId}, Project: {ProjectId}, Group: {GroupId}",
                @event.RegistrationId, @event.ProjectId, @event.GroupId);
            // TODO: Notify group about confirmation
            return Task.CompletedTask;
        }
    }
}
