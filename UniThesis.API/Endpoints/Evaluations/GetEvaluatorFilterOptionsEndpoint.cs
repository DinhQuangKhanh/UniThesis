using Microsoft.EntityFrameworkCore;
using UniThesis.Application.Common;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.API.Endpoints.Evaluations;

public class GetEvaluatorFilterOptionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/evaluator/filter-options", async (
                AppDbContext context,
                ILogger<GetEvaluatorFilterOptionsEndpoint> logger,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var semesters = await context.Semesters
                        .AsNoTracking()
                        .OrderByDescending(s => s.StartDate)
                        .Select(s => new { value = s.Id, label = s.Name })
                        .ToListAsync(cancellationToken);

                    var majors = await context.Majors
                        .AsNoTracking()
                        .Where(m => m.IsActive)
                        .OrderBy(m => m.Name)
                        .Select(m => new { value = m.Id, label = m.Name })
                        .ToListAsync(cancellationToken);

                    return Results.Ok(ApiResponse.Ok(new { semesters, majors }));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi tải dữ liệu bộ lọc (Học kỳ, Chuyên ngành)");
                    return Results.Json(ApiResponse.Fail("Không thể tải dữ liệu bộ lọc (Học kỳ, Chuyên ngành). Vui lòng thử lại sau."), statusCode: 500);
                }
            })
            .RequireAuthorization()
            .WithTags("Evaluator")
            .WithName("GetEvaluatorFilterOptions")
            .Produces(200)
            .Produces(401);
    }
}
