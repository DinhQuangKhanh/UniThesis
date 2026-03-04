using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Notifications.DTOs;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Application.Features.Notifications.Queries.GetUserNotifications;

/// <summary>
/// Handles GetUserNotificationsQuery by fetching notifications from MongoDB
/// via INotificationService for the current authenticated user.
/// </summary>
public class GetUserNotificationsQueryHandler
    : IQueryHandler<GetUserNotificationsQuery, NotificationListResponseDto>
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUser;

    public GetUserNotificationsQueryHandler(
        INotificationService notificationService,
        ICurrentUserService currentUser)
    {
        _notificationService = notificationService;
        _currentUser = currentUser;
    }

    public async Task<NotificationListResponseDto> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var userId = _currentUser.UserId.Value;

        var notifications = await _notificationService.GetUserNotificationsAsync(userId, request.Limit, cancellationToken);
        var unreadCount = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);

        var items = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Content = n.Content,
            Type = n.Type.ToString(),
            Category = n.Category.ToString(),
            TargetUrl = n.TargetUrl,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        });

        return new NotificationListResponseDto
        {
            Items = items,
            TotalCount = items.Count(),
            UnreadCount = unreadCount
        };
    }
}
