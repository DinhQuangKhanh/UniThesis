using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.SemesterAggregate.Events;

namespace UniThesis.Application.Features.Semesters.EventHandlers;

/// <summary>
/// Handles the SemesterCreatedEvent domain event.
/// </summary>
public class SemesterCreatedEventHandler : INotificationHandler<SemesterCreatedEvent>
{
    private readonly ILogger<SemesterCreatedEventHandler> _logger;

    public SemesterCreatedEventHandler(ILogger<SemesterCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SemesterCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Semester created: Id={SemesterId}, Code={SemesterCode}",
            notification.SemesterId,
            notification.SemesterCode);

        return Task.CompletedTask;
    }
}
