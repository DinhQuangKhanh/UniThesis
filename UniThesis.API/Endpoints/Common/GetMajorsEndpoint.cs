using Microsoft.EntityFrameworkCore;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.API.Endpoints.Common;

public class GetMajorsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/majors", async (
                AppDbContext context,
                CancellationToken cancellationToken) =>
            {
                var majors = await context.Majors
                    .AsNoTracking()
                    .Where(m => m.IsActive)
                    .OrderBy(m => m.Name)
                    .Select(m => new { m.Id, m.Name, m.Code })
                    .ToListAsync(cancellationToken);

                return Results.Ok(majors);
            })
            .RequireAuthorization()
            .WithTags("Common")
            .WithName("GetMajors")
            .Produces(200)
            .Produces(401);
    }
}
