using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Persistence.Services
{
  /// <summary>
  /// Persists MediatR request pipeline logs into <c>user_activity_logs</c>
  /// as <see cref="UserActivityLogDocument"/> with Category = "Request".
  /// </summary>
  public class RequestLogService : IRequestLogService
  {
    private readonly IUserActivityLogRepository _activityLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RequestLogService> _logger;

    public RequestLogService(
        IUserActivityLogRepository activityLogRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<RequestLogService> logger)
    {
      _activityLogRepository = activityLogRepository;
      _httpContextAccessor = httpContextAccessor;
      _logger = logger;
    }

    public async Task LogAsync(RequestLogEntry entry, CancellationToken cancellationToken = default)
    {
      try
      {
        _ = Guid.TryParse(entry.UserId, out var userId);

        var http = _httpContextAccessor.HttpContext;
        var ipAddress = http?.Connection.RemoteIpAddress?.ToString();
        var userAgent = http?.Request.Headers["User-Agent"].ToString();

        var document = new UserActivityLogDocument
        {
          UserId = userId,
          UserName = entry.UserName ?? entry.UserEmail ?? "Anonymous",
          UserEmail = entry.UserEmail,
          UserRole = ResolveActiveRole(entry.UserRole),
          Action = entry.RequestName,
          Category = "Request",
          Severity = entry.IsSuccess ? "info" : "error",
          IpAddress = ipAddress,
          UserAgent = userAgent,
          Timestamp = entry.Timestamp,
          Details = BuildDetails(entry),
        };

        await _activityLogRepository.AddAsync(document, cancellationToken);
      }
      catch (Exception ex)
      {
        // Never let log persistence failures bubble up and break the request pipeline.
        _logger.LogError(ex, "Failed to persist request log for {RequestName} to MongoDB", entry.RequestName);
      }
    }

    /// <summary>
    /// Determines the user's active role based on the HTTP request path.
    /// Endpoints follow the convention /api/{role}/... so we can extract the role from the path.
    /// Falls back to the role from JWT claims if the path doesn't match any known role prefix.
    /// </summary>
    private string ResolveActiveRole(string? fallbackRole)
    {
      var path = _httpContextAccessor.HttpContext?.Request.Path.Value;
      if (path != null)
      {
        if (path.StartsWith("/api/evaluator/", StringComparison.OrdinalIgnoreCase)) return "evaluator";
        if (path.StartsWith("/api/mentor/", StringComparison.OrdinalIgnoreCase)) return "mentor";
        if (path.StartsWith("/api/admin/", StringComparison.OrdinalIgnoreCase)) return "admin";
        if (path.StartsWith("/api/student/", StringComparison.OrdinalIgnoreCase)) return "student";
      }
      return fallbackRole ?? "anonymous";
    }

    private static BsonDocument BuildDetails(RequestLogEntry entry)
    {
      var details = new BsonDocument
      {
        ["IsSuccess"] = entry.IsSuccess,
        ["ElapsedMilliseconds"] = entry.ElapsedMilliseconds,
        ["ErrorMessage"] = entry.ErrorMessage ?? BsonNull.Value.ToString(),
        ["ErrorType"] = entry.ErrorType ?? BsonNull.Value.ToString(),
      };

      if (entry.RequestParameters is { Count: > 0 })
      {
        var paramDoc = new BsonDocument();
        foreach (var (key, value) in entry.RequestParameters)
        {
          paramDoc[key] = value switch
          {
            null      => BsonNull.Value,
            int i     => new BsonInt32(i),
            long l    => new BsonInt64(l),
            bool b    => new BsonBoolean(b),
            double d  => new BsonDouble(d),
            Guid g    => new BsonString(g.ToString()),
            DateTime dt => new BsonDateTime(dt),
            _         => new BsonString(value.ToString() ?? string.Empty),
          };
        }
        details["RequestParameters"] = paramDoc;
      }

      return details;
    }
  }
}
