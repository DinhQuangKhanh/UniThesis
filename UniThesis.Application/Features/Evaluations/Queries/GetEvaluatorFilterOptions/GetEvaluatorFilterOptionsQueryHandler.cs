using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorFilterOptions;

public class GetEvaluatorFilterOptionsQueryHandler
    : IQueryHandler<GetEvaluatorFilterOptionsQuery, EvaluatorFilterOptionsDto>
{
    private readonly IEvaluatorQueryService _queryService;

    public GetEvaluatorFilterOptionsQueryHandler(IEvaluatorQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<EvaluatorFilterOptionsDto> Handle(
        GetEvaluatorFilterOptionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _queryService.GetFilterOptionsAsync(cancellationToken);
    }
}
