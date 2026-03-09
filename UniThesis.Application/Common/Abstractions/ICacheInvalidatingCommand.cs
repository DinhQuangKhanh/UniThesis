namespace UniThesis.Application.Common.Abstractions;

/// <summary>
/// Marker interface for commands that should invalidate cache entries after successful execution.
/// The CacheInvalidationBehavior will clear all specified prefixes from both L1 and L2 cache.
/// </summary>
public interface ICacheInvalidatingCommand : ICommand
{
    /// <summary>
    /// Cache key prefixes to invalidate from both L1 and L2 after command succeeds.
    /// Example: ["evaluator:{evaluatorId}:", "evaluator:filter-options"]
    /// </summary>
    IReadOnlyCollection<string> CachePrefixesToInvalidate { get; }
}
