using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Common;
using UniThesis.Application.Features.Evaluations.DTOs;
using UniThesis.Application.Features.Evaluations.Queries.CheckTitleSimilarity;
using UniThesis.Infrastructure.Authorization.Policies;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Evaluations;

public class CheckTitleSimilarityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/evaluator/projects/{projectId:guid}/similarity", async (
                Guid projectId,
                ISender sender,
                ILogger<CheckTitleSimilarityEndpoint> logger,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var query = new CheckTitleSimilarityQuery(projectId);
                    var result = await sender.Send(query, cancellationToken);
                    return Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Json(ApiResponse.Fail(ex.Message), statusCode: 404);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi kiểm tra trùng lặp tiêu đề");
                    return Results.Json(ApiResponse.Fail("Không thể kiểm tra trùng lặp. Vui lòng thử lại sau."), statusCode: 500);
                }
            })
            .RequireAuthorization(PolicyNames.RequireEvaluator)
            .WithTags("Evaluator")
            .WithName("CheckTitleSimilarity")
            .Produces<ApiResponse<List<SimilarTitleDto>>>()
            .Produces(404);
    }
}
