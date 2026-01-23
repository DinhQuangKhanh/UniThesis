using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Enums.Evaluation;

namespace UniThesis.Domain.Specifications.Evaluations
{
    public class SubmissionsByEvaluatorSpec : BaseSpecification<EvaluationSubmission>
    {
        public SubmissionsByEvaluatorSpec(Guid evaluatorId, SubmissionStatus? status = null)
            : base(s => s.AssignedEvaluatorId == evaluatorId &&
                        (!status.HasValue || s.Status == status.Value))
        {
            ApplyOrderByDescending(s => s.AssignedAt!);
        }
    }
}
