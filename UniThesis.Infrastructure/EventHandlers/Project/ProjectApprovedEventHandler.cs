using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectApprovedEventHandler : INotificationHandler<ProjectApprovedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<ProjectApprovedEventHandler> _logger;

        public ProjectApprovedEventHandler(INotificationService notificationService, ILogger<ProjectApprovedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(ProjectApprovedEvent notification, CancellationToken cancellationToken)
        {
            // Notification would be sent with project name - simplified here
            _logger.LogInformation("Project approved: {ProjectId}", notification.ProjectId);
            await Task.CompletedTask;
        }
    }
}
