using Microsoft.Extensions.Logging;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.BackgroundJobs.Jobs
{
    public class MeetingReminderJob
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MeetingReminderJob> _logger;

        public MeetingReminderJob(INotificationService notificationService, ILogger<MeetingReminderJob> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Starting MeetingReminderJob");
            // Implementation: Send reminders for upcoming meetings
            await Task.CompletedTask;
            _logger.LogInformation("MeetingReminderJob completed");
        }
    }
}
