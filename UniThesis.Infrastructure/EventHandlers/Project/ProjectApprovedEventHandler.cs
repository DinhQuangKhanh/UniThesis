using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectApprovedEventHandler : IDomainEventHandler<ProjectApprovedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<ProjectApprovedEventHandler> _logger;

        public ProjectApprovedEventHandler(INotificationService notificationService, ILogger<ProjectApprovedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(ProjectApprovedEvent @event, CancellationToken ct = default)
        {
            // Notification would be sent with project name - simplified here
            _logger.LogInformation("Project approved: {ProjectId}", @event.ProjectId);
            await Task.CompletedTask;
        }
    }
}
