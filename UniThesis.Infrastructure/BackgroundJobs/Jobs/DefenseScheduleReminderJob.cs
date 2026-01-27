using Microsoft.Extensions.Logging;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.BackgroundJobs.Jobs
{
    public class DefenseScheduleReminderJob
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<DefenseScheduleReminderJob> _logger;

        public DefenseScheduleReminderJob(INotificationService notificationService, ILogger<DefenseScheduleReminderJob> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Starting DefenseScheduleReminderJob");
            // Implementation: Send reminders for upcoming defense schedules
            await Task.CompletedTask;
            _logger.LogInformation("DefenseScheduleReminderJob completed");
        }
    }
}
