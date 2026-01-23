using UniThesis.Domain.Aggregates.ProjectAggregate;

namespace UniThesis.Domain.Specifications.Projects
{
    public class ProjectByGroupSpec : BaseSpecification<Project>
    {
        public ProjectByGroupSpec(Guid groupId) : base(p => p.GroupId == groupId)
        {
            AddInclude(p => p.Mentors);
            AddInclude(p => p.Documents);
        }
    }
}
