using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.SupportAggregate.Events;
using UniThesis.Domain.Enums.Notification;

namespace UniThesis.Application.Features.Supports.EventHandlers;

/// <summary>
/// Handles the TicketClosedEvent domain event.
/// Logs the closure and notifies the reporter.
/// </summary>
public class TicketClosedEventHandler : INotificationHandler<TicketClosedEvent>
{
    private readonly ILogger<TicketClosedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly ISupportTicketRepository _ticketRepository;

    public TicketClosedEventHandler(
        ILogger<TicketClosedEventHandler> logger,
        INotificationService notificationService,
        ISupportTicketRepository ticketRepository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _ticketRepository = ticketRepository;
    }

    public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket closed: TicketId={TicketId}",
            notification.TicketId);

        var ticket = await _ticketRepository.GetByIdAsync(notification.TicketId, cancellationToken);
        if (ticket is null) return;

        // Notify the reporter that their ticket has been closed
        await _notificationService.SendAsync(
            ticket.ReporterId,
            "Ticket đã được đóng",
            $"Ticket {ticket.Code.Value}: \"{ticket.Title}\" đã được đóng. Cảm ơn bạn đã sử dụng hệ thống hỗ trợ.",
            NotificationType.Info,
            NotificationCategory.Support,
            $"/supports/{notification.TicketId}",
            cancellationToken);
    }
}
