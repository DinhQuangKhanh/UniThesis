using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Evaluations.Commands.SubmitEvaluation;

[ActionLog("Submit Evaluation", "Evaluation")]
public record SubmitEvaluationCommand(Guid ProjectId, int Result, string? Feedback) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
        ["evaluator:{userId}:"];
}
