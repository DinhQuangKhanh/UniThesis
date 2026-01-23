using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Specifications.TopicPools
{
    public class ExpiringTopicsSpec : BaseSpecification<TopicPool>
    {
        public ExpiringTopicsSpec(int currentSemesterId)
            : base(t => t.Status == TopicPoolStatus.Available &&
                        t.ExpirationSemesterId <= currentSemesterId + 1)
        {
            ApplyOrderBy(t => t.ExpirationSemesterId);
        }
    }
}
