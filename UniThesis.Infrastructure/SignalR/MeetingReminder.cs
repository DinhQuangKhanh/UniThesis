namespace UniThesis.Infrastructure.SignalR
{
    /// <summary>
    /// Meeting reminder model.
    /// </summary>
    public record MeetingReminder(
        Guid MeetingId,
        string Title,
        DateTime ScheduledTime,
        string Location,
        int MinutesUntilStart
    );
}
