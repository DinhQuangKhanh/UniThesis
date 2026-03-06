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
          UserRole = entry.UserRole ?? "anonymous",
          Action = entry.RequestName,
          Category = "Request",
          Severity = entry.IsSuccess ? "info" : "error",
          IpAddress = ipAddress,
          UserAgent = userAgent,
          Timestamp = entry.Timestamp,
          Details = new BsonDocument
          {
            ["IsSuccess"] = entry.IsSuccess,
            ["ElapsedMilliseconds"] = entry.ElapsedMilliseconds,
            ["ErrorMessage"] = entry.ErrorMessage ?? BsonNull.Value.ToString(),
            ["ErrorType"] = entry.ErrorType ?? BsonNull.Value.ToString(),
          },
        };

        await _activityLogRepository.AddAsync(document, cancellationToken);
      }
      catch (Exception ex)
      {
        // Never let log persistence failures bubble up and break the request pipeline.
        _logger.LogError(ex, "Failed to persist request log for {RequestName} to MongoDB", entry.RequestName);
      }
    }
  }
}
