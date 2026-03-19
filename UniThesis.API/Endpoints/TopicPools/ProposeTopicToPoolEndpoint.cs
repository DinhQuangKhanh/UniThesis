using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.TopicPools.Commands.ProposeTopicToPool;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.TopicPools;

/// <summary>
/// Endpoint: POST /api/topic-pools/{poolId}/propose
/// Allows a mentor to propose a new topic into a topic pool.
/// </summary>
public class ProposeTopicToPoolEndpoint : IEndpoint
{
    public sealed record ProposeTopicRequest(
        string NameVi,
        string NameEn,
        string NameAbbr,
        string Description,
        string Objectives,
        string? Scope,
        string? Technologies,
        string? ExpectedResults,
        int MaxStudents = 5);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/topic-pools/{poolId:guid}/propose", async (
                Guid poolId,
                ProposeTopicRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
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
                return Created($"/api/topic-pools/topics/{projectId}", new { id = projectId }, "Tạo mới thành công.");
            })
            .RequireAuthorization()
            .WithTags("TopicPools")
            .WithName("ProposeTopicToPool")
            .Produces<object>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }
}
