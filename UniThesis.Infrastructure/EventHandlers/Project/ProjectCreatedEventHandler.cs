using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Project
{
    public class ProjectCreatedEventHandler : IDomainEventHandler<ProjectCreatedEvent>
    {
        private readonly ILogger<ProjectCreatedEventHandler> _logger;

        public ProjectCreatedEventHandler(ILogger<ProjectCreatedEventHandler> logger) => _logger = logger;

        public Task HandleAsync(ProjectCreatedEvent @event, CancellationToken ct = default)
        {
            _logger.LogInformation("Project created: {ProjectId}, Code: {ProjectCode}", @event.ProjectId, @event.ProjectCode);
            return Task.CompletedTask;
        }
    }
}
