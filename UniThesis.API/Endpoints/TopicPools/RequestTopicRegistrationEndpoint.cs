using MediatR;
using UniThesis.Application.Features.TopicPools.Commands.RequestRegistration;

namespace UniThesis.API.Endpoints.TopicPools;

/// <summary>
/// Endpoint: POST /api/topic-pools/registrations
/// Allows a student group to request registration for a topic from the pool.
/// </summary>
public class RequestTopicRegistrationEndpoint : IEndpoint
{
    public sealed record TopicRegistrationRequest(Guid ProjectId, Guid GroupId, string? Note);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/topic-pools/registrations", async (
                TopicRegistrationRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new RequestTopicRegistrationCommand(
                    body.ProjectId,
                    body.GroupId,
                    body.Note);

                var registrationId = await sender.Send(command, cancellationToken);
                return Results.Created($"/api/topic-pools/registrations/{registrationId}", new { id = registrationId });
            })
            .RequireAuthorization()
            .WithTags("TopicPools")
            .WithName("RequestTopicRegistration")
            .Produces<object>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}
