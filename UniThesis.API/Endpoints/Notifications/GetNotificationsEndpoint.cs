using MediatR;
using UniThesis.Application.Features.Notifications.DTOs;
using UniThesis.Application.Features.Notifications.Queries.GetUserNotifications;

namespace UniThesis.API.Endpoints.Notifications;

/// <summary>
/// Endpoint: GET /api/notifications
/// Returns paginated notifications for the authenticated user.
/// </summary>
public class GetNotificationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications", async (
                ISender sender,
                int limit = 50,
                CancellationToken cancellationToken = default) =>
            {
                if (limit is < 1 or > 200) limit = 50;

                var result = await sender.Send(
                    new GetUserNotificationsQuery(limit), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithTags("Notifications")
            .WithName("GetNotifications")
            .Produces<NotificationListResponseDto>()
            .Produces(401);
    }
}
