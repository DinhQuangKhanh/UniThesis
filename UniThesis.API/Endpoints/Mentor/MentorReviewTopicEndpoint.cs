using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.DirectRegistration.Commands.MentorReviewTopic;
using UniThesis.Infrastructure.Authorization.Policies;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Mentor;

public class MentorReviewTopicEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/mentor/topics/{projectId:guid}/review", async (
                Guid projectId,
                MentorReviewRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new MentorReviewTopicCommand(projectId, request.Action, request.Feedback);
                await sender.Send(command, cancellationToken);
                return NoContent("Đã xử lý đề tài thành công.");
            })
             .RequireAuthorization(PolicyNames.MentorOfProject)
            .WithTags("Mentor")
            .WithName("MentorReviewTopic")
            .Produces(200)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}

public record MentorReviewRequest(string Action, string? Feedback);
