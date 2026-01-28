using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace UniThesis.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior for logging request execution time and details.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly Interfaces.ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        Interfaces.ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId?.ToString() ?? "Anonymous";
        var userEmail = _currentUserService.Email ?? "N/A";

        _logger.LogInformation(
            "Starting request {RequestName} by User {UserId} ({UserEmail})",
            requestName, userId, userEmail);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Completed request {RequestName} in {ElapsedMilliseconds}ms",
                requestName, stopwatch.ElapsedMilliseconds);

            // Log warning for slow requests (> 500ms)
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "Long running request {RequestName} ({ElapsedMilliseconds}ms) by User {UserId}",
                    requestName, stopwatch.ElapsedMilliseconds, userId);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(
                ex,
                "Request {RequestName} failed after {ElapsedMilliseconds}ms for User {UserId}",
                requestName, stopwatch.ElapsedMilliseconds, userId);
            
            throw;
        }
    }
}
