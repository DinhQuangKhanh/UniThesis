using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Common;
using UniThesis.Application.Features.Evaluations.DTOs;
using UniThesis.Application.Features.Evaluations.Queries.GetProjectForReview;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Evaluations;

public class GetProjectForReviewEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/evaluator/projects/{projectId:guid}/review", async (
                Guid projectId,
                ISender sender,
                ILogger<GetProjectForReviewEndpoint> logger,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var query = new GetProjectForReviewQuery(projectId);
                    var dto = await sender.Send(query, cancellationToken);

                    if (dto is null)
                        return Results.Json(ApiResponse.Fail("Không tìm thấy đề tài hoặc bạn không có quyền thẩm định."), statusCode: 404);

                    return Ok(dto);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Json(ApiResponse.Fail("Bạn chưa đăng nhập hoặc phiên đã hết hạn."), statusCode: 401);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi tải thông tin đề tài để thẩm định");
                    return Results.Json(ApiResponse.Fail("Không thể tải thông tin đề tài. Vui lòng thử lại sau."), statusCode: 500);
                }
            })
            .RequireAuthorization()
            .WithTags("Evaluator")
            .WithName("GetProjectForReview")
            .Produces<ApiResponse<ProjectReviewDetailDto>>()
            .Produces(401)
            .Produces(404);
    }
}
