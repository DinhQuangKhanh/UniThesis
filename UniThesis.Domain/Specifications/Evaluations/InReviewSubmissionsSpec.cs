using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Enums.Evaluation;

namespace UniThesis.Domain.Specifications.Evaluations
{
    public class InReviewSubmissionsSpec : BaseSpecification<EvaluationSubmission>
    {
        public InReviewSubmissionsSpec(Guid? evaluatorId = null)
            : base(s => s.Status == SubmissionStatus.InReview &&
                        (!evaluatorId.HasValue || s.AssignedEvaluatorId == evaluatorId.Value))
        {
            ApplyOrderBy(s => s.AssignedAt!);
        }
    }
}
