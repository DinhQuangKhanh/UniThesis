using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using UniThesis.Application.Common;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

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
                await LogExceptionToActivityLogAsync(context, exception);
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

        private async Task LogExceptionToActivityLogAsync(HttpContext context, Exception exception)
        {
            try
            {
                var repository = context.RequestServices.GetService<IUserActivityLogRepository>();
                if (repository is null) return;

                var user = context.User;
                _ = Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);

                var path = context.Request.Path.Value ?? "";
                var role = ResolveRoleFromPath(path);

                var document = new UserActivityLogDocument
                {
                    UserId = userId,
                    UserName = user.FindFirstValue(ClaimTypes.Name)
                               ?? user.FindFirstValue("name")
                               ?? "Anonymous",
                    UserEmail = user.FindFirstValue(ClaimTypes.Email),
                    UserRole = role,
                    Action = "UnhandledException",
                    Category = "System",
                    Severity = "critical",
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    Timestamp = DateTime.UtcNow,
                    Details = new BsonDocument
                    {
                        ["ErrorMessage"] = exception.Message,
                        ["ErrorType"] = exception.GetType().FullName,
                        ["StackTrace"] = exception.StackTrace ?? string.Empty,
                        ["RequestPath"] = path,
                        ["RequestMethod"] = context.Request.Method,
                    },
                };

                await repository.AddAsync(document);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to persist unhandled exception to activity log");
            }
        }

        private static string ResolveRoleFromPath(string path)
        {
            if (path.StartsWith("/api/admin/", StringComparison.OrdinalIgnoreCase)) return "admin";
            if (path.StartsWith("/api/mentor/", StringComparison.OrdinalIgnoreCase)) return "mentor";
            if (path.StartsWith("/api/evaluator/", StringComparison.OrdinalIgnoreCase)) return "evaluator";
            if (path.StartsWith("/api/student/", StringComparison.OrdinalIgnoreCase)) return "student";
            return "system";
        }
    }
}
