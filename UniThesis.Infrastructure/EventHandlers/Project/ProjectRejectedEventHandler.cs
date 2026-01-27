using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectRejectedEventHandler : IDomainEventHandler<ProjectRejectedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<ProjectRejectedEventHandler> _logger;

        public ProjectRejectedEventHandler(INotificationService notificationService, ILogger<ProjectRejectedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(ProjectRejectedEvent @event, CancellationToken ct = default)
        {
            _logger.LogInformation("Project rejected: {ProjectId}", @event.ProjectId);
            await Task.CompletedTask;
        }
    }
}
