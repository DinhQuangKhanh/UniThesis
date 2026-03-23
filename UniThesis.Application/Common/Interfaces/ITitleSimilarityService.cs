using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Common.Interfaces;

public interface ITitleSimilarityService
{
    Task<List<SimilarTitleDto>> FindSimilarTitlesAsync(
        Guid projectId,
        int topN = 3,
        CancellationToken cancellationToken = default);
}
