namespace UniThesis.Infrastructure.SignalR
{
    /// <summary>
    /// Project status update model.
    /// </summary>
    public record ProjectStatusUpdate(
        Guid ProjectId,
        string ProjectName,
        string OldStatus,
        string NewStatus,
        DateTime UpdatedAt
    );
}
