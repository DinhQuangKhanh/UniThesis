using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorProjects;

public record GetEvaluatorProjectsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    int? SemesterId = null,
    int? MajorId = null,
    string? Result = null
) : IQuery<EvaluatorProjectsDto>;
