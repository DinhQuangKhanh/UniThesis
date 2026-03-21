using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.CheckTitleSimilarity;

public class CheckTitleSimilarityQueryHandler : IQueryHandler<CheckTitleSimilarityQuery, List<SimilarTitleDto>>
{
    private readonly ITitleSimilarityService _similarityService;

    public CheckTitleSimilarityQueryHandler(ITitleSimilarityService similarityService)
    {
        _similarityService = similarityService;
    }

    public async Task<List<SimilarTitleDto>> Handle(CheckTitleSimilarityQuery request, CancellationToken cancellationToken)
    {
        return await _similarityService.FindSimilarTitlesAsync(request.ProjectId, topN: 3, cancellationToken: cancellationToken);
    }
}
