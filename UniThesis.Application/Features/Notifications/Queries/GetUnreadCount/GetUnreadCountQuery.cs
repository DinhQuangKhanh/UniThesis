using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.Notifications.Queries.GetUnreadCount;

/// <summary>
/// Query to get the count of unread notifications for the current user.
/// </summary>
public record GetUnreadCountQuery(UniThesis.Domain.Enums.Notification.NotificationCategory? Category = null) : IQuery<long>;
