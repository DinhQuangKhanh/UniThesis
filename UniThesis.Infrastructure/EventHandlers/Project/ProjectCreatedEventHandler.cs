using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
    {
        private readonly ILogger<ProjectCreatedEventHandler> _logger;

        public ProjectCreatedEventHandler(ILogger<ProjectCreatedEventHandler> logger) => _logger = logger;

        public Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Project created: {ProjectId}, Code: {ProjectCode}", notification.ProjectId, notification.ProjectCode);
            return Task.CompletedTask;
        }
    }
}
