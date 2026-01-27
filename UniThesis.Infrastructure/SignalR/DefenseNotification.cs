namespace UniThesis.Infrastructure.SignalR
{
    /// <summary>
    /// Defense notification model.
    /// </summary>
    public record DefenseNotification(
        Guid DefenseId,
        Guid ProjectId,
        string ProjectName,
        DateTime ScheduledTime,
        string Location,
        string Role
    );

}
