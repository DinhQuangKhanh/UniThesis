using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Domain.Specifications.Projects
{
    public class ProjectPendingEvaluationSpec : BaseSpecification<Project>
    {
        public ProjectPendingEvaluationSpec(int? semesterId = null)
            : base(p => p.Status == ProjectStatus.PendingEvaluation &&
                        (!semesterId.HasValue || p.SemesterId == semesterId.Value))
        {
            AddInclude(p => p.Mentors);
            ApplyOrderBy(p => p.SubmittedAt!);
        }
    }
}
