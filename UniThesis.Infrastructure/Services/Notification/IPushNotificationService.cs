namespace UniThesis.Infrastructure.Services.Notification
{
    /// <summary>
    /// Interface for push notification services (Firebase, APNs, etc.)
    /// </summary>
    public interface IPushNotificationService
    {
        /// <summary>
        /// Sends a push notification to a single device.
        /// </summary>
        /// <param name="deviceToken">The device's push notification token.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="body">The notification body.</param>
        /// <param name="data">Additional data payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if sent successfully.</returns>
        Task<bool> SendToDeviceAsync(
            string deviceToken,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a push notification to multiple devices.
        /// </summary>
        /// <param name="deviceTokens">The device tokens.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="body">The notification body.</param>
        /// <param name="data">Additional data payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of successfully sent notifications.</returns>
        Task<int> SendToDevicesAsync(
            IEnumerable<string> deviceTokens,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a push notification to a topic (all subscribed devices).
        /// </summary>
        /// <param name="topic">The topic name.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="body">The notification body.</param>
        /// <param name="data">Additional data payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if sent successfully.</returns>
        Task<bool> SendToTopicAsync(
            string topic,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a push notification to a user (all their registered devices).
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="body">The notification body.</param>
        /// <param name="data">Additional data payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of successfully sent notifications.</returns>
        Task<int> SendToUserAsync(
            Guid userId,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Subscribes a device to a topic.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="topic">The topic name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task SubscribeToTopicAsync(string deviceToken, string topic, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsubscribes a device from a topic.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="topic">The topic name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task UnsubscribeFromTopicAsync(string deviceToken, string topic, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a device token for a user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="platform">The device platform (ios, android, web).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task RegisterDeviceAsync(Guid userId, string deviceToken, string platform, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unregisters a device token.
        /// </summary>
        /// <param name="deviceToken">The device token to unregister.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task UnregisterDeviceAsync(string deviceToken, CancellationToken cancellationToken = default);
    }
}
