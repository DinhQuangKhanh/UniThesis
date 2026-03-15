using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Notifications.Commands.MarkAsRead;

/// <summary>
/// Command to mark a specific notification as read.
/// </summary>
[ActionLog("Mark Notification Read", "Notification")]
public record MarkNotificationAsReadCommand(Guid NotificationId) : ICommand;
