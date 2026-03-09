using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Notifications.Commands.MarkAsRead;

/// <summary>
/// Command to mark a specific notification as read.
/// </summary>
public record MarkNotificationAsReadCommand(Guid NotificationId) : ICommand;
