using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace UniThesis.Infrastructure.Logging
{
    /// <summary>
    /// Telemetry processor to filter out health check requests.
    /// </summary>
    internal class HealthCheckTelemetryFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public HealthCheckTelemetryFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            // Filter out health check requests
            if (item is RequestTelemetry request)
            {
                if (IsHealthCheckRequest(request.Url?.AbsolutePath))
                {
                    return; // Don't send this telemetry
                }
            }

            // Filter out dependency calls to health endpoints
            if (item is DependencyTelemetry dependency)
            {
                if (IsHealthCheckRequest(dependency.Data))
                {
                    return;
                }
            }

            _next.Process(item);
        }

        private static bool IsHealthCheckRequest(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            var healthCheckPaths = new[]
            {
            "/health",
            "/healthz",
            "/ready",
            "/live",
            "/ping",
            "/api/health"
        };

            return healthCheckPaths.Any(hcp => path.Contains(hcp, StringComparison.OrdinalIgnoreCase));
        }
    }
}
