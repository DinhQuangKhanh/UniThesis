using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using UniThesis.Domain.Common.Exceptions;

namespace UniThesis.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, response) = exception switch
            {
                
                NotFoundException notFoundEx => (HttpStatusCode.NotFound, new ErrorResponse("NotFound", notFoundEx.Message)),
                Domain.Common.Exceptions.ValidationException validationEx => (HttpStatusCode.BadRequest, new ErrorResponse("ValidationError", validationEx.Message, validationEx.Errors)),
                UnauthorizedAccessException => (HttpStatusCode.Forbidden, new ErrorResponse("Forbidden", "You don't have permission to access this resource.")),
                DomainException domainEx => (HttpStatusCode.BadRequest, new ErrorResponse("DomainError", domainEx.Message)),
                _ => (HttpStatusCode.InternalServerError, new ErrorResponse("InternalError", "An unexpected error occurred."))
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning("Handled exception: {Type} - {Message}", exception.GetType().Name, exception.Message);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
        }
    }

    public record ErrorResponse(string Code, string Message, Dictionary<string, string[]>? Errors = null);
}
