using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Specifications.TopicPools
{
    public class TopicsByMajorSpec : BaseSpecification<TopicPool>
    {
        public TopicsByMajorSpec(int majorId, bool availableOnly = true)
            : base(t => t.MajorId == majorId &&
                        (!availableOnly || t.Status == TopicPoolStatus.Available))
        {
            ApplyOrderByDescending(t => t.CreatedAt);
        }
    }
}
