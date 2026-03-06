using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.SemesterAggregate.Events;

namespace UniThesis.Application.Features.Semesters.EventHandlers;

/// <summary>
/// Handles the SemesterClosedEvent domain event.
/// </summary>
public class SemesterClosedEventHandler : INotificationHandler<SemesterClosedEvent>
{
    private readonly ILogger<SemesterClosedEventHandler> _logger;

    public SemesterClosedEventHandler(ILogger<SemesterClosedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SemesterClosedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Semester closed: Id={SemesterId}",
            notification.SemesterId);

        return Task.CompletedTask;
    }
}
