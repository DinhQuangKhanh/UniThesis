using UniThesis.Domain.Aggregates.EvaluationAggregate;

namespace UniThesis.Domain.Specifications.Evaluations
{
    public class SubmissionsByProjectSpec : BaseSpecification<EvaluationSubmission>
    {
        public SubmissionsByProjectSpec(Guid projectId) : base(s => s.ProjectId == projectId)
        {
            ApplyOrderBy(s => s.SubmissionNumber);
        }
    }
}
