namespace UniThesis.API.Common.Security;

internal interface ITopicProposalAttachmentScanWorkflow
{
    Task<TopicProposalAttachmentQueueResult> QueueAsync(
    Guid projectId,
        Guid poolId,
    Guid uploadedBy,
        IReadOnlyCollection<IFormFile>? attachments,
        CancellationToken cancellationToken = default);
}

internal sealed record TopicProposalAttachmentQueueResult(
    bool Success,
    int QueuedCount,
    string? ErrorMessage = null)
{
    public static TopicProposalAttachmentQueueResult Ok(int queuedCount) => new(true, queuedCount);

    public static TopicProposalAttachmentQueueResult Failed(string errorMessage, int queuedCount = 0)
        => new(false, queuedCount, errorMessage);
}
