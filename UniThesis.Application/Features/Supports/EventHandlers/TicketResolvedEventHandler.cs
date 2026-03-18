using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.SupportAggregate.Events;
using UniThesis.Domain.Enums.Notification;

namespace UniThesis.Application.Features.Supports.EventHandlers;

/// <summary>
/// Handles the TicketResolvedEvent domain event.
/// Logs the resolution and notifies the reporter.
/// </summary>
public class TicketResolvedEventHandler : INotificationHandler<TicketResolvedEvent>
{
    private readonly ILogger<TicketResolvedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly ISupportTicketRepository _ticketRepository;

    public TicketResolvedEventHandler(
        ILogger<TicketResolvedEventHandler> logger,
        INotificationService notificationService,
        ISupportTicketRepository ticketRepository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _ticketRepository = ticketRepository;
    }

    public async Task Handle(TicketResolvedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket resolved: TicketId={TicketId}",
            notification.TicketId);

        var ticket = await _ticketRepository.GetByIdAsync(notification.TicketId, cancellationToken);
        if (ticket is null) return;

        // Notify the reporter that their ticket has been resolved
        await _notificationService.SendAsync(
            ticket.ReporterId,
            "Ticket đã được giải quyết",
            $"Ticket {ticket.Code.Value}: \"{ticket.Title}\" đã được giải quyết. Nếu bạn vẫn còn vấn đề, vui lòng phản hồi lại.",
            NotificationType.Success,
            NotificationCategory.Support,
            $"/supports/{notification.TicketId}",
            cancellationToken);
    }
}
