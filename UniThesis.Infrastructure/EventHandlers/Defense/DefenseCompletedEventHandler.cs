using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.DefenseAggregate.Events;

namespace UniThesis.Infrastructure.EventHandlers.Defense
{
    public class DefenseCompletedEventHandler : INotificationHandler<DefenseCompletedEvent>
    {
        private readonly ILogger<DefenseCompletedEventHandler> _logger;

        public DefenseCompletedEventHandler(ILogger<DefenseCompletedEventHandler> logger) => _logger = logger;

        public Task Handle(DefenseCompletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Defense completed: {DefenseId}", notification.DefenseId);
            return Task.CompletedTask;
        }
    }
}
