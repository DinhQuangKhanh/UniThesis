using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.SupportAggregate.Events;
using UniThesis.Domain.Enums.Notification;

namespace UniThesis.Application.Features.Supports.EventHandlers;

/// <summary>
/// Handles the TicketStatusChangedEvent domain event.
/// Logs the status change and notifies the reporter.
/// </summary>
public class TicketStatusChangedEventHandler : INotificationHandler<TicketStatusChangedEvent>
{
    private readonly ILogger<TicketStatusChangedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly ISupportTicketRepository _ticketRepository;

    public TicketStatusChangedEventHandler(
        ILogger<TicketStatusChangedEventHandler> logger,
        INotificationService notificationService,
        ISupportTicketRepository ticketRepository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _ticketRepository = ticketRepository;
    }

    public async Task Handle(TicketStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket status changed: TicketId={TicketId}, OldStatus={OldStatus}, NewStatus={NewStatus}",
            notification.TicketId,
            notification.OldStatus,
            notification.NewStatus);

        var ticket = await _ticketRepository.GetByIdAsync(notification.TicketId, cancellationToken);
        if (ticket is null) return;

        // Notify the reporter about the status change
        await _notificationService.SendAsync(
            ticket.ReporterId,
            "Cập nhật trạng thái ticket",
            $"Ticket {ticket.Code.Value} đã được chuyển từ \"{notification.OldStatus}\" sang \"{notification.NewStatus}\".",
            NotificationType.Info,
            NotificationCategory.Support,
            $"/supports/{notification.TicketId}",
            cancellationToken);
    }
}
