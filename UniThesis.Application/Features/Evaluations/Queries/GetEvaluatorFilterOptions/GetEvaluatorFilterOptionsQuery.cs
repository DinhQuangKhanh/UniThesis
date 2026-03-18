using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorFilterOptions;

public record GetEvaluatorFilterOptionsQuery() : ICachedQuery<EvaluatorFilterOptionsDto>
{
    public string CacheKey => "evaluator:filter-options";
    public TimeSpan? L1Expiration => TimeSpan.FromMinutes(30);
    public TimeSpan? L2Expiration => TimeSpan.FromHours(2);
}
