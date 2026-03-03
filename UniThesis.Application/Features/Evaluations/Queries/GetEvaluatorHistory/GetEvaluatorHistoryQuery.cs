using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorHistory;

public record GetEvaluatorHistoryQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? Result = null,
    string? DateRange = null
) : IQuery<EvaluatorHistoryDto>;
