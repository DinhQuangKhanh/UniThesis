using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetProjectForReview;

public record GetProjectForReviewQuery(Guid ProjectId) : IQuery<ProjectReviewDetailDto?>;
