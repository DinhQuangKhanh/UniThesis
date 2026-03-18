using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Notifications.Commands.MarkAllAsRead;

/// <summary>
/// Command to mark all notifications as read for the current user.
/// </summary>
[ActionLog("Mark All Notifications Read", "Notification")]
public record MarkAllNotificationsAsReadCommand() : ICommand;
