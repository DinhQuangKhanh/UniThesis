using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using UniThesis.Domain.Aggregates.MeetingAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Notification;
using UniThesis.Infrastructure.Services.Notification;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Meeting
{
    /// <summary>
    /// Handler for MeetingCompletedEvent.
    /// </summary>
    public class MeetingCompletedEventHandler : IDomainEventHandler<MeetingCompletedEvent>
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

        public async Task HandleAsync(MeetingCompletedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Meeting completed: MeetingId={MeetingId}, ProjectId={ProjectId}",
                @event.MeetingId, @event.ProjectId);

            try
            {
                // Log the activity
                var log = new UserActivityLogDocument
                {
                    UserId = Guid.Empty,
                    Action = "MeetingCompleted",
                    EntityType = "Meeting",
                    EntityId = @event.MeetingId,
                    Timestamp = DateTime.UtcNow,
                    Details = new BsonDocument
                    {
                        ["MeetingId"] = new BsonBinaryData(@event.MeetingId.ToByteArray()),
                        ["ProjectId"] = new BsonBinaryData(@event.ProjectId.ToByteArray()),
                    }
                };

                await _activityLogRepository.AddAsync(log, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MeetingCompletedEvent for meeting {MeetingId}", @event.MeetingId);
            }
        }
    }
}
