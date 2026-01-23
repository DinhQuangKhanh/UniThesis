using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Domain.Specifications.Projects
{
    public class ProjectNeedsModificationSpec : BaseSpecification<Project>
    {
        public ProjectNeedsModificationSpec(Guid? mentorId = null)
            : base(p => p.Status == ProjectStatus.NeedsModification &&
                        (!mentorId.HasValue || p.Mentors.Any(m => m.MentorId == mentorId.Value && m.IsActive)))
        {
            AddInclude(p => p.Mentors);
            ApplyOrderBy(p => p.UpdatedAt!);
        }
    }
}
