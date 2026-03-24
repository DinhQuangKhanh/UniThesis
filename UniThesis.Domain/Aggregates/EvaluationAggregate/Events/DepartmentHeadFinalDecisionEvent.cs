using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Evaluation;

namespace UniThesis.Domain.Aggregates.EvaluationAggregate.Events
{
    /// <summary>
    /// Domain event raised when the Department Head makes a final decision
    /// on a project where the two evaluators gave conflicting results.
    /// </summary>
    public sealed record DepartmentHeadFinalDecisionEvent(
        Guid ProjectId,
        EvaluationResult FinalResult,
        Guid DecidedBy
    ) : DomainEventBase;
}
