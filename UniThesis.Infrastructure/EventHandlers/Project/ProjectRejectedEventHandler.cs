using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectRejectedEventHandler : INotificationHandler<ProjectRejectedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<ProjectRejectedEventHandler> _logger;

        public ProjectRejectedEventHandler(INotificationService notificationService, ILogger<ProjectRejectedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task Handle(ProjectRejectedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Project rejected: {ProjectId}", notification.ProjectId);
            return Task.CompletedTask;
        }
    }
}
