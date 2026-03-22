using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using UniThesis.API.Common.Security;
using UniThesis.API.Extensions;
using UniThesis.Application.Common;
using UniThesis.Application.Common.Interfaces;
using UniThesis.API.Endpoints.TopicPools.Requests;
using UniThesis.Application.Features.TopicPools.Commands.ProposeTopicToPool;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.TopicPools;

/// <summary>
/// Endpoint: POST /api/topic-pools/{poolId}/propose
/// Allows a mentor to propose a new topic into a topic pool.
/// </summary>
public class ProposeTopicToPoolEndpoint : IEndpoint
{
    private const long ProposeTopicMaxUploadBytes = 25 * 1024 * 1024; // 25 MB
    private const long ProposeTopicPerFileMaxUploadBytes = 10 * 1024 * 1024; // 10 MB / file
    private const int ProposeTopicMaxAttachmentCount = 5;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/topic-pools/{poolId:guid}/propose", async (
                Guid poolId,
                [FromForm] ProposeTopicRequest body,
                HttpContext httpContext,
                ITopicProposalAttachmentScanWorkflow attachmentScanWorkflow,
                ICurrentUserService currentUser,
                ILogger<ProposeTopicToPoolEndpoint> logger,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var hasFormContentType = httpContext.Request.HasFormContentType;
                var requestFormFilesCount = hasFormContentType ? httpContext.Request.Form.Files.Count : 0;
                var modelAttachmentsCount = body.Attachments?.Count ?? 0;
                var effectiveAttachments = modelAttachmentsCount > 0
                    ? body.Attachments
                    : requestFormFilesCount > 0
                        ? [.. httpContext.Request.Form.Files]
                        : null;
                var effectiveAttachmentsCount = effectiveAttachments?.Count ?? 0;

                logger.LogInformation(
                    "ProposeTopic multipart diagnostics: HasFormContentType={HasFormContentType}, Request.Form.Files.Count={RequestFormFilesCount}, Body.Attachments.Count={ModelAttachmentsCount}, EffectiveAttachments.Count={EffectiveAttachmentsCount}",
                    hasFormContentType,
                    requestFormFilesCount,
                    modelAttachmentsCount,
                    effectiveAttachmentsCount);

                if (!TopicProposalAttachmentValidator.TryValidate(
                        effectiveAttachments,
                        perFileMaxBytes: ProposeTopicPerFileMaxUploadBytes,
                        maxAttachmentCount: ProposeTopicMaxAttachmentCount,
                        out var attachmentError))
                {
                    return Results.BadRequest(ApiResponse.Fail(attachmentError));
                }

                var command = new ProposeTopicToPoolCommand(
                    PoolId: poolId,
                    NameVi: body.NameVi,
                    NameEn: body.NameEn,
                    NameAbbr: body.NameAbbr,
                    Description: body.Description,
                    Objectives: body.Objectives,
                    Scope: body.Scope,
                    Technologies: body.Technologies,
                    ExpectedResults: body.ExpectedResults,
                    MaxStudents: body.MaxStudents);

                var projectId = await sender.Send(command, cancellationToken);

                var queueResult = await attachmentScanWorkflow.QueueAsync(
                    projectId,
                    poolId,
                    currentUser.UserId ?? Guid.Empty,
                    effectiveAttachments,
                    cancellationToken);

                var message = queueResult.QueuedCount > 0
                    ? $"Tạo mới thành công. Có {queueResult.QueuedCount} tệp đang chờ quét mã độc trong nền."
                    : queueResult.Success
                        ? "Tạo mới thành công. Không có tệp đính kèm để quét."
                        : "Tạo mới thành công, nhưng chưa thể đưa tệp đính kèm vào hàng đợi quét mã độc.";

                return Created($"/api/topic-pools/topics/{projectId}", new { id = projectId }, message);
            })
            .WithTags("TopicPools")
            .WithName("ProposeTopicToPool")
            .Produces<object>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .WithMetadata(new RequestSizeLimitAttribute(ProposeTopicMaxUploadBytes))
            .WithMetadata(new RequestFormLimitsAttribute { MultipartBodyLengthLimit = ProposeTopicMaxUploadBytes })
            .DisableAntiforgery()
            .WithRequestTimeout("ProposeTopicUploadTimeout")
            .RequireRateLimiting("ProposeTopicUploadPolicy")
            .RequireAuthorization();
    }
}
