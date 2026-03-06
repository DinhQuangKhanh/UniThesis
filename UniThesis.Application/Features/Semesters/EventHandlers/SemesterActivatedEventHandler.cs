using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.SemesterAggregate.Events;

namespace UniThesis.Application.Features.Semesters.EventHandlers;

/// <summary>
/// Handles the SemesterActivatedEvent domain event.
/// </summary>
public class SemesterActivatedEventHandler : INotificationHandler<SemesterActivatedEvent>
{
    private readonly ILogger<SemesterActivatedEventHandler> _logger;

    public SemesterActivatedEventHandler(ILogger<SemesterActivatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SemesterActivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Semester activated: Id={SemesterId}",
            notification.SemesterId);

        return Task.CompletedTask;
    }
}
