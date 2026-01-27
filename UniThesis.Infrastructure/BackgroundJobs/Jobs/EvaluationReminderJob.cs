using Microsoft.Extensions.Logging;
using UniThesis.Domain.Services;
using UniThesis.Infrastructure.Services.Notification;

namespace UniThesis.Infrastructure.BackgroundJobs.Jobs
{
    public class EvaluationReminderJob
    {
        private readonly IEvaluationDomainService _evaluationService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<EvaluationReminderJob> _logger;

        public EvaluationReminderJob(
            IEvaluationDomainService evaluationService,
            INotificationService notificationService,
            ILogger<EvaluationReminderJob> logger)
        {
            _evaluationService = evaluationService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Starting EvaluationReminderJob");
            // Implementation: Send reminders for pending evaluations
            await Task.CompletedTask;
            _logger.LogInformation("EvaluationReminderJob completed");
        }
    }
}
