using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Departments.Commands.SubmitFinalDecision;

[ActionLog("Submit Final Decision", "Department")]
public record SubmitFinalDecisionCommand(Guid ProjectId, int Result, string? Notes) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
    [
        "department-head:",
        "evaluator:"
    ];
}
