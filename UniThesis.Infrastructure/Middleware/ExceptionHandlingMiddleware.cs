using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using UniThesis.Application.Common;
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
                EntityNotFoundException entityNotFoundEx => (HttpStatusCode.NotFound, ApiResponse.Fail(entityNotFoundEx.Message)),
                Domain.Common.Exceptions.ValidationException validationEx => (HttpStatusCode.BadRequest, ApiResponse.Fail(validationEx.Message, validationEx.Errors)),
                BusinessRuleValidationException businessRuleEx => (HttpStatusCode.BadRequest, ApiResponse.Fail(businessRuleEx.Message)),
                ConcurrencyException concurrencyEx => (HttpStatusCode.Conflict, ApiResponse.Fail(concurrencyEx.Message)),
                UnauthorizedAccessException => (HttpStatusCode.Forbidden, ApiResponse.Fail("Bạn không có quyền truy cập tài nguyên này.")),
                DomainException domainEx => (HttpStatusCode.BadRequest, ApiResponse.Fail(domainEx.Message)),
                _ => (HttpStatusCode.InternalServerError, ApiResponse.Fail("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau."))
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
}
