using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Notifications.Commands.MarkAllAsRead;

/// <summary>
/// Command to mark all notifications as read for the current user.
/// </summary>
public record MarkAllNotificationsAsReadCommand() : ICommand;
