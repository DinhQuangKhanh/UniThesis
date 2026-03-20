using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniThesis.API.Endpoints.Supports.Requests;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Supports.Commands.CreateTicket;
using UniThesis.Application.Features.Supports.Commands.ReplyTicket;
using UniThesis.Application.Features.Supports.Commands.UpdateTicketStatus;
using UniThesis.Application.Features.Supports.Queries.GetTicketById;
using UniThesis.Application.Features.Supports.Queries.GetTickets;
using UniThesis.Application.Features.Supports.Queries.GetTicketStats;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.Supports;

public class SupportEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/supports").RequireAuthorization();

        // 1. Thống kê tổng quan ticket (Admin only typically, but we allow based on permissions later if needed)
        group.MapGet("stats", async (ISender sender) =>
        {
            var result = await sender.Send(new GetTicketStatsQuery());
            return Ok(result);
        })
        .WithName("GetTicketStats")
        .WithTags("Supports");

        // 2. Danh sách ticket (Filter, Search)
        group.MapGet("", async ([AsParameters] GetTicketsRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetTicketsQuery(request.SearchTerm, request.Status, request.Priority));
            return Ok(result);
        })
        .WithName("GetTickets")
        .WithTags("Supports");

        // 3. Chi tiết ticket
        group.MapGet("{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetTicketByIdQuery(id));
            return Ok(result);
        })
        .WithName("GetTicketById")
        .WithTags("Supports");

        // 4. Tạo ticket mới
        group.MapPost("", async ([FromBody] CreateTicketRequest request, ISender sender, HttpContext context) =>
        {
            var reporterId = context.User.GetUserId();
            var command = new CreateTicketCommand(
                request.Title,
                request.Description,
                request.Category,
                request.Priority,
                reporterId);

            var ticketId = await sender.Send(command);
            return Created($"/api/supports/{ticketId}", new { Id = ticketId }, "Tạo mới thành công.");
        })
        .WithName("CreateTicket")
        .WithTags("Supports");

        // 5. Phản hồi ticket
        group.MapPost("{id:guid}/reply", async (Guid id, [FromBody] ReplyTicketRequest request, ISender sender, HttpContext context) =>
        {
            var senderId = context.User.GetUserId();
            var command = new ReplyTicketCommand(id, senderId, request.Content);
            await sender.Send(command);

            // Auto update to InProgress if an admin replies and it's open
            if (context.User.IsInRole("Admin"))
            {
                // We'll optimistically send an update command. Domain handles ignoring if not open.
                await sender.Send(new UpdateTicketStatusCommand(id, Domain.Enums.Ticket.TicketStatus.InProgress));
            }

            return Ok("Phản hồi thành công.");
        })
        .WithName("ReplyTicket")
        .WithTags("Supports");

        // 6. Cập nhật trạng thái ticket
        group.MapPatch("{id:guid}/status", async (Guid id, [FromBody] UpdateTicketStatusRequest request, ISender sender) =>
        {
            var command = new UpdateTicketStatusCommand(id, request.Status);
            await sender.Send(command);
            return Ok("Cập nhật thành công.");
        })
        .WithName("UpdateTicketStatus")
        .WithTags("Supports");
    }
}
