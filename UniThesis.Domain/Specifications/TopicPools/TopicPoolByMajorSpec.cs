using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Specifications.TopicPools;

/// <summary>
/// Specification to get topic pool by major.
/// Each major has exactly one permanent topic pool.
/// </summary>
public class TopicPoolByMajorSpec : BaseSpecification<TopicPool>
{
    /// <summary>
    /// Gets the topic pool for a specific major.
    /// </summary>
    /// <param name="majorId">Major ID</param>
    public TopicPoolByMajorSpec(int majorId)
        : base(tp => tp.MajorId == majorId)
    {
    }
}
