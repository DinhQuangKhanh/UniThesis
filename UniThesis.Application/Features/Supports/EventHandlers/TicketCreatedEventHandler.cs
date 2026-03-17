using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.SupportAggregate.Events;
using UniThesis.Domain.Enums.Notification;

namespace UniThesis.Application.Features.Supports.EventHandlers;

/// <summary>
/// Handles the TicketCreatedEvent domain event.
/// Logs ticket creation and sends a notification to all admins.
/// </summary>
public class TicketCreatedEventHandler : INotificationHandler<TicketCreatedEvent>
{
    private readonly ILogger<TicketCreatedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public TicketCreatedEventHandler(
        ILogger<TicketCreatedEventHandler> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Support ticket created: TicketId={TicketId}, Code={TicketCode}, Category={Category}, Priority={Priority}",
            notification.TicketId,
            notification.TicketCode,
            notification.Category,
            notification.Priority);

        // Notify admins about the new ticket
        await _notificationService.SendAsync(
            Guid.Empty, // System-level notification — actual admin targeting is handled by NotificationService
            "Yêu cầu hỗ trợ mới",
            $"Ticket {notification.TicketCode} (Ưu tiên: {notification.Priority}) vừa được tạo.",
            NotificationType.Info,
            NotificationCategory.Support,
            $"/admin/supports/{notification.TicketId}",
            cancellationToken);
    }
}
