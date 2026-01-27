using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.DefenseAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Defense
{
    public class DefenseCompletedEventHandler : IDomainEventHandler<DefenseCompletedEvent>
    {
        private readonly ILogger<DefenseCompletedEventHandler> _logger;

        public DefenseCompletedEventHandler(ILogger<DefenseCompletedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(DefenseCompletedEvent @event, CancellationToken ct = default)
        {
            _logger.LogInformation("Defense completed: {DefenseId}", @event.DefenseId);
            return Task.CompletedTask;
        }
    }
}
