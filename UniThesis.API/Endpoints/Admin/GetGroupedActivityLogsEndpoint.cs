using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.API.Endpoints.Admin;

public class GetGroupedActivityLogsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/activity-logs/grouped", async (
                IUserActivityLogRepository repository,
                string? role,
                string? severity,
                string? search,
                DateTime? from,
                DateTime? to,
                int page = 1,
                int pageSize = 20,
                CancellationToken cancellationToken = default) =>
            {
                if (page < 1) page = 1;
                if (pageSize is < 1 or > 100) pageSize = 20;

                var (items, totalGroups, roleCounts) = await repository.GetGroupedAsync(
                    role, severity, search, from, to, page, pageSize, cancellationToken);

                var result = new
                {
                    Items = items.Select(i => new
                    {
                        UserId = i.UserId.ToString(),
                        i.UserName,
                        i.UserEmail,
                        i.UserRole,
                        i.Action,
                        i.Category,
                        i.TotalCount,
                        i.LatestTimestamp,
                        i.SeverityCounts,
                    }),
                    TotalGroups = totalGroups,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalGroups / pageSize),
                    RoleCounts = roleCounts,
                };

                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithTags("Admin")
            .WithName("GetGroupedActivityLogs")
            .Produces(200)
            .Produces(401);
    }
}
