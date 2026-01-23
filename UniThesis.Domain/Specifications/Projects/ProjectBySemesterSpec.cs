using UniThesis.Domain.Aggregates.ProjectAggregate;

namespace UniThesis.Domain.Specifications.Projects
{
    public class ProjectBySemesterSpec : BaseSpecification<Project>
    {
        public ProjectBySemesterSpec(int semesterId, bool includeDetails = false) : base(p => p.SemesterId == semesterId)
        {
            if (includeDetails)
            {
                AddInclude(p => p.Mentors);
                AddInclude(p => p.Documents);
            }
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }
}
