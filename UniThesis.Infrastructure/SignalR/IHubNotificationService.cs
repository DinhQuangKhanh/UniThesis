namespace UniThesis.Infrastructure.SignalR
{
    public interface IHubNotificationService
    {
        Task SendToUserAsync(Guid userId, string method, object message, CancellationToken cancellationToken = default);
        Task SendToUsersAsync(IEnumerable<Guid> userIds, string method, object message, CancellationToken cancellationToken = default);
        Task SendToGroupAsync(string groupName, string method, object message, CancellationToken cancellationToken = default);
        Task SendToAllAsync(string method, object message, CancellationToken cancellationToken = default);
    }
}
