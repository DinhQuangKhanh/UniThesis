using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Notifications.Queries.GetUnreadCount;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Notifications;

/// <summary>
/// Endpoint: GET /api/notifications/unread-count
/// Returns the count of unread notifications for the authenticated user.
/// </summary>
public class GetUnreadCountEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications/unread-count", async (
                ISender sender,
                CancellationToken cancellationToken = default) =>
            {
                var count = await sender.Send(new GetUnreadCountQuery(), cancellationToken);
                return Ok(new { UnreadCount = count });
            })
            .RequireAuthorization()
            .WithTags("Notifications")
            .WithName("GetUnreadCount")
            .Produces(200)
            .Produces(401);
    }
}
