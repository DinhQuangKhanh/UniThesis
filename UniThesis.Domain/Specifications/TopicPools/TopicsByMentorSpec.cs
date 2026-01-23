using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Specifications.TopicPools
{
    public class TopicsByMentorSpec : BaseSpecification<TopicPool>
    {
        public TopicsByMentorSpec(Guid mentorId, TopicPoolStatus? status = null)
            : base(t => t.ProposedBy == mentorId &&
                        (!status.HasValue || t.Status == status.Value))
        {
            ApplyOrderByDescending(t => t.CreatedAt);
        }
    }
}
