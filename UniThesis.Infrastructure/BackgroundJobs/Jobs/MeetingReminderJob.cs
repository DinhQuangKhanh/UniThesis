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

        public Task ExecuteAsync()
        {
            _logger.LogInformation("Starting MeetingReminderJob");
            // TODO: Implementation: Send reminders for upcoming meetings
            _logger.LogInformation("MeetingReminderJob completed");
            return Task.CompletedTask;
        }
    }
}
