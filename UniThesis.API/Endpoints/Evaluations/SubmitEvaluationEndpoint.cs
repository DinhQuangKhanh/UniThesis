using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Common;
using UniThesis.Application.Features.Evaluations.Commands.SubmitEvaluation;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Evaluations;

public class SubmitEvaluationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/evaluator/projects/{projectId:guid}/evaluate", async (
                Guid projectId,
                SubmitEvaluationRequest body,
                ISender sender,
                ILogger<SubmitEvaluationEndpoint> logger,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new SubmitEvaluationCommand(projectId, body.Result, body.Feedback);
                    await sender.Send(command, cancellationToken);
                    return Ok<string>("Thẩm định đã được gửi thành công.");
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Json(ApiResponse.Fail("Bạn chưa đăng nhập hoặc phiên đã hết hạn."), statusCode: 401);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Json(ApiResponse.Fail(ex.Message), statusCode: 400);
                }
                catch (ArgumentException ex)
                {
                    return Results.Json(ApiResponse.Fail(ex.Message), statusCode: 400);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi gửi thẩm định");
                    return Results.Json(ApiResponse.Fail("Không thể gửi thẩm định. Vui lòng thử lại sau."), statusCode: 500);
                }
            })
            .RequireAuthorization()
            .WithTags("Evaluator")
            .WithName("SubmitEvaluation")
            .Produces<ApiResponse<string>>()
            .Produces(400)
            .Produces(401);
    }
}

public record SubmitEvaluationRequest(int Result, string? Feedback);
