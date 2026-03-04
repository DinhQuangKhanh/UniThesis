namespace UniThesis.Application.Common.Interfaces;

/// <summary>
/// Service for persisting MediatR request execution logs to a durable store.
/// </summary>
public interface IRequestLogService
{
  /// <summary>
  /// Persists a log entry for a completed or failed request.
  /// </summary>
  Task LogAsync(RequestLogEntry entry, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a single MediatR request log entry.
/// </summary>
public record RequestLogEntry(
    string RequestName,
    string? UserId,
    string? UserName,
    string? UserEmail,
    string? UserRole,
    bool IsSuccess,
    long ElapsedMilliseconds,
    DateTime Timestamp,
    string? ErrorMessage = null,
    string? ErrorType = null
);
