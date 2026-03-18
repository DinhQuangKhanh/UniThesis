using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniThesis.Application.Features.Semesters.Commands.CreateSemester;
using UniThesis.Application.Features.Semesters.Commands.DeleteSemester;
using UniThesis.Application.Features.Semesters.Commands.UpdateSemester;
using UniThesis.Application.Features.Semesters.Queries.GetActiveSemester;
using UniThesis.Application.Features.Semesters.Queries.GetAllSemesters;
using UniThesis.Application.Features.Semesters.Queries.GetSemesterById;

namespace UniThesis.API.Endpoints.Semesters;

public class GetAllSemestersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/semesters", async (
                [FromQuery] string? status,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllSemestersQuery(status), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Semesters")
            .WithName("GetAllSemesters")
            .Produces(200)
            .Produces(401);
    }
}

public class GetSemesterByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/semesters/{id:int}", async (
                int id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetSemesterByIdQuery(id), cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Semesters")
            .WithName("GetSemesterById")
            .Produces(200)
            .Produces(401)
            .Produces(404);
    }
}

public class GetActiveSemesterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/semesters/active", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetActiveSemesterQuery(), cancellationToken);
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Semesters")
            .WithName("GetActiveSemester")
            .Produces(200)
            .Produces(401)
            .Produces(404);
    }
}

public class CreateSemesterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/admin/semesters", async (
                CreateSemesterCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var id = await sender.Send(command, cancellationToken);
                return Results.Created($"/api/admin/semesters/{id}", new { id });
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Semesters")
            .WithName("CreateSemester")
            .Produces(201)
            .Produces(400)
            .Produces(401);
    }
}


public class UpdateSemesterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/admin/semesters/{id:int}", async (
                int id,
                UpdateSemesterCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (id != command.Id) return Results.BadRequest("Id mismatch");
                await sender.Send(command, cancellationToken);
                return Results.NoContent();
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Semesters")
            .WithName("UpdateSemester")
            .Produces(204)
            .Produces(400)
            .Produces(401)
            .Produces(404);
    }
}

public class DeleteSemesterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/admin/semesters/{id:int}", async (
                int id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                await sender.Send(new DeleteSemesterCommand(id), cancellationToken);
                return Results.NoContent();
            })
            .RequireAuthorization("RequireAdmin")
            .WithTags("Semesters")
            .WithName("DeleteSemester")
            .Produces(204)
            .Produces(400)
            .Produces(401)
            .Produces(404);
    }
}
