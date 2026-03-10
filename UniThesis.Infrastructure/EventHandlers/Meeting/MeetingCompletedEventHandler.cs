using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using UniThesis.Domain.Aggregates.MeetingAggregate.Events;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Meeting
{
    /// <summary>
    /// Handler for MeetingCompletedEvent.
    /// </summary>
    public class MeetingCompletedEventHandler : INotificationHandler<MeetingCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ILogger<MeetingCompletedEventHandler> _logger;

        public MeetingCompletedEventHandler(
            INotificationService notificationService,
            IUserActivityLogRepository activityLogRepository,
            ILogger<MeetingCompletedEventHandler> logger)
        {
            _notificationService = notificationService;
            _activityLogRepository = activityLogRepository;
            _logger = logger;
        }

        public async Task Handle(MeetingCompletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Meeting completed: MeetingId={MeetingId}, ProjectId={ProjectId}",
                notification.MeetingId, notification.ProjectId);

            try
            {
                // Log the activity
                var log = new UserActivityLogDocument
                {
                    UserId   = Guid.Empty,
                    UserName = "System",
                    UserRole = "system",
                    Action   = "MeetingCompleted",
                    Category = "Meeting",
                    EntityType = "Meeting",
                    EntityId = notification.MeetingId,
                    Severity = "info",
                    Timestamp = DateTime.UtcNow,
                    Details = new BsonDocument
                    {
                        ["MeetingId"] = new BsonBinaryData(notification.MeetingId.ToByteArray()),
                        ["ProjectId"] = new BsonBinaryData(notification.ProjectId.ToByteArray()),
                    }
                };

                await _activityLogRepository.AddAsync(log, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MeetingCompletedEvent for meeting {MeetingId}", notification.MeetingId);
            }
        }
    }
}
