using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorDashboard;

public class GetEvaluatorDashboardQueryHandler : IQueryHandler<GetEvaluatorDashboardQuery, EvaluatorDashboardDto>
{
    private readonly IEvaluatorQueryService _queryService;
    private readonly ICurrentUserService _currentUser;

    public GetEvaluatorDashboardQueryHandler(
        IEvaluatorQueryService queryService,
        ICurrentUserService currentUser)
    {
        _queryService = queryService;
        _currentUser = currentUser;
    }

    public async Task<EvaluatorDashboardDto> Handle(
        GetEvaluatorDashboardQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var evaluatorId = _currentUser.UserId.Value;

        return await _queryService.GetDashboardAsync(evaluatorId, cancellationToken);
    }
}
