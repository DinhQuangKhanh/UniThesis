using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetProjectForReview;

public class GetProjectForReviewQueryHandler : IQueryHandler<GetProjectForReviewQuery, ProjectReviewDetailDto?>
{
    private readonly IEvaluatorQueryService _queryService;
    private readonly ICurrentUserService _currentUser;

    public GetProjectForReviewQueryHandler(IEvaluatorQueryService queryService, ICurrentUserService currentUser)
    {
        _queryService = queryService;
        _currentUser = currentUser;
    }

    public async Task<ProjectReviewDetailDto?> Handle(GetProjectForReviewQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        return await _queryService.GetProjectForReviewAsync(request.ProjectId, _currentUser.UserId.Value, cancellationToken);
    }
}
