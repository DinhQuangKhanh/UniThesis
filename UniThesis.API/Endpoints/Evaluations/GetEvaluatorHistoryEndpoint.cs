using MediatR;
using UniThesis.Application.Common;
using UniThesis.Application.Features.Evaluations.DTOs;
using UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorHistory;

namespace UniThesis.API.Endpoints.Evaluations;

public class GetEvaluatorHistoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/evaluator/history", async (
                ISender sender,
                ILogger<GetEvaluatorHistoryEndpoint> logger,
                int page = 1,
                int pageSize = 10,
                string? search = null,
                string? result = null,
                string? dateRange = null,
                CancellationToken cancellationToken = default) =>
            {
                try
                {
                    var query = new GetEvaluatorHistoryQuery(page, pageSize, search, result, dateRange);
                    var dto = await sender.Send(query, cancellationToken);
                    return Results.Ok(ApiResponse.Ok(dto));
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Json(ApiResponse.Fail("Bạn chưa đăng nhập hoặc phiên đã hết hạn."), statusCode: 401);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi tải lịch sử thẩm định");
                    return Results.Json(ApiResponse.Fail("Không thể tải lịch sử thẩm định. Vui lòng thử lại sau."), statusCode: 500);
                }
            })
            .RequireAuthorization()
            .WithTags("Evaluator")
            .WithName("GetEvaluatorHistory")
            .Produces<ApiResponse<EvaluatorHistoryDto>>()
            .Produces(401);
    }
}
