using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorProjects;

public class GetEvaluatorProjectsQueryHandler : IQueryHandler<GetEvaluatorProjectsQuery, EvaluatorProjectsDto>
{
    private readonly IEvaluatorQueryService _queryService;
    private readonly ICurrentUserService _currentUser;

    public GetEvaluatorProjectsQueryHandler(
        IEvaluatorQueryService queryService,
        ICurrentUserService currentUser)
    {
        _queryService = queryService;
        _currentUser = currentUser;
    }

    public async Task<EvaluatorProjectsDto> Handle(
        GetEvaluatorProjectsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        return await _queryService.GetProjectsAsync(
            _currentUser.UserId.Value,
            request.Page,
            request.PageSize,
            request.Search,
            request.SemesterId,
            request.MajorId,
            request.Result,
            cancellationToken);
    }
}
