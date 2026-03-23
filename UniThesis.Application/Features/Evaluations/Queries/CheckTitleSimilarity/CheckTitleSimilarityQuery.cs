using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.CheckTitleSimilarity;

public record CheckTitleSimilarityQuery(Guid ProjectId) : IQuery<List<SimilarTitleDto>>;
