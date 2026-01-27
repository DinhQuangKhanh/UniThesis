using Microsoft.Extensions.Logging;

namespace UniThesis.Infrastructure.Logging
{
    /// <summary>
    /// Extension methods for Application Insights logging.
    /// </summary>
    public static class ApplicationInsightsLoggerExtensions
    {
        /// <summary>
        /// Tracks a custom event in Application Insights.
        /// </summary>
        public static void TrackEvent(this ILogger logger, string eventName, IDictionary<string, string>? properties = null)
        {
            var telemetryClient = GetTelemetryClient();
            telemetryClient?.TrackEvent(eventName, properties);

            logger.LogInformation("Custom event tracked: {EventName}", eventName);
        }

        /// <summary>
        /// Tracks a custom metric in Application Insights.
        /// </summary>
        public static void TrackMetric(this ILogger logger, string metricName, double value, IDictionary<string, string>? properties = null)
        {
            var telemetryClient = GetTelemetryClient();
            telemetryClient?.TrackMetric(metricName, value, properties);

            logger.LogDebug("Custom metric tracked: {MetricName} = {Value}", metricName, value);
        }

        private static Microsoft.ApplicationInsights.TelemetryClient? GetTelemetryClient()
        {
            // Note: In real implementation, get this from DI
            return null;
        }
    }
}
