using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Specifications.TopicPools
{
    public class AvailableTopicsSpec : BaseSpecification<TopicPool>
    {
        public AvailableTopicsSpec(int? majorId = null)
            : base(t => t.Status == TopicPoolStatus.Available &&
                        (!majorId.HasValue || t.MajorId == majorId.Value))
        {
            ApplyOrderByDescending(t => t.CreatedAt);
        }
    }
}
