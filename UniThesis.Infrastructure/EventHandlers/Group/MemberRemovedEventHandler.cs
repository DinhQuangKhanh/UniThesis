using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Notification;
using UniThesis.Infrastructure.Services.Notification;
using UniThesis.Persistence.MongoDB.Documents;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;

namespace UniThesis.Infrastructure.EventHandlers.Group
{
    public class MemberRemovedEventHandler : IDomainEventHandler<MemberRemovedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ILogger<MemberRemovedEventHandler> _logger;

        public MemberRemovedEventHandler(
            INotificationService notificationService,
            IUserActivityLogRepository activityLogRepository,
            ILogger<MemberRemovedEventHandler> logger)
        {
            _notificationService = notificationService;
            _activityLogRepository = activityLogRepository;
            _logger = logger;
        }

        public async Task HandleAsync(MemberRemovedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Member removed from group: GroupId={GroupId}, StudentId={StudentId}, RemovedBy={RemovedBy}",
                @event.GroupId, @event.StudentId, @event.RemovedBy);

            try
            {
                // Log the activity
                var log = new UserActivityLogDocument
                {
                    UserId = @event.RemovedBy,
                    Action = "MemberRemoved",
                    EntityType = "Group",
                    EntityId = @event.GroupId,
                    Timestamp = DateTime.UtcNow,
                    Details = new BsonDocument
                    {
                        ["GroupId"] = new BsonBinaryData(@event.GroupId.ToByteArray()),
                        ["RemovedStudentId"] = new BsonBinaryData(@event.StudentId.ToByteArray()),
                    }
                };

                await _activityLogRepository.AddAsync(log, cancellationToken);

                // Notify the removed student
                await _notificationService.SendAsync(
                    @event.StudentId,
                    "Bạn đã bị xóa khỏi nhóm",
                    "Bạn đã bị xóa khỏi nhóm đồ án. Vui lòng liên hệ nhóm trưởng để biết thêm chi tiết.",
                    NotificationType.Warning,
                    NotificationCategory.Group,
                    "/groups",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MemberRemovedEvent for group {GroupId}", @event.GroupId);
            }
        }
    }
}
