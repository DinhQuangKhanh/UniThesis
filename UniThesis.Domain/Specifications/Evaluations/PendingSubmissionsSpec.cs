using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Enums.Evaluation;

namespace UniThesis.Domain.Specifications.Evaluations
{
    public class PendingSubmissionsSpec : BaseSpecification<EvaluationSubmission>
    {
        public PendingSubmissionsSpec()
            : base(s => s.Status == SubmissionStatus.Pending)
        {
            ApplyOrderBy(s => s.SubmittedAt);
        }
    }
}
