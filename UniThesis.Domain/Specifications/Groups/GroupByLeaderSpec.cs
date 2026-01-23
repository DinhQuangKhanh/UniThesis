using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Enums.Group;

namespace UniThesis.Domain.Specifications.Groups
{
    public class GroupByLeaderSpec : BaseSpecification<Group>
    {
        public GroupByLeaderSpec(Guid leaderId)
            : base(g => g.LeaderId == leaderId && g.Status == GroupStatus.Active)
        {
            AddInclude(g => g.Members);
        }
    }
}
