using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Domain.Specifications.Projects
{
    public class ProjectByStatusSpec : BaseSpecification<Project>
    {
        public ProjectByStatusSpec(ProjectStatus status) : base(p => p.Status == status)
        {
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }
}