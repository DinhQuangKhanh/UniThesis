using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Evaluation;

namespace UniThesis.Domain.Aggregates.EvaluationAggregate.Events
{
    /// <summary>
    /// Domain event raised when an evaluator submits their individual evaluation result.
    /// </summary>
    public sealed record EvaluatorSubmittedResultEvent(
        Guid AssignmentId,
        Guid ProjectId,
        Guid EvaluatorId,
        EvaluationResult Result
    ) : DomainEventBase;
}
