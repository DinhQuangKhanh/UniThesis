using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Evaluation;

namespace UniThesis.Domain.Aggregates.EvaluationAggregate.Events
{
    public sealed record EvaluationCompletedEvent(Guid SubmissionId, Guid ProjectId, EvaluationResult Result) : DomainEventBase;
}
