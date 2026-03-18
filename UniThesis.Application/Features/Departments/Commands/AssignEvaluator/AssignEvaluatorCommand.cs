using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.Departments.Commands.AssignEvaluator;

/// <summary>
/// Command for DepartmentHead (CNBM) to assign an evaluator to a project.
/// The CNBM can only assign evaluators to projects that belong to their department.
/// Business rules enforced:
/// - Evaluator cannot evaluate a project they are mentoring
/// - Maximum 3 evaluators per project
/// </summary>
[ActionLog("Assign Evaluator", "Department")]
public record AssignEvaluatorCommand(
    Guid ProjectId,
    Guid EvaluatorId,
    int EvaluatorOrder
) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate =>
    [
        $"evaluator:{EvaluatorId}:",
        "evaluator:filter-options"
    ];
}
