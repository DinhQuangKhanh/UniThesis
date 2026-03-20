using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetMyPendingJoinRequest;

public class GetMyPendingJoinRequestQueryHandler : IQueryHandler<GetMyPendingJoinRequestQuery, PendingJoinRequestDto?>
{
    private readonly IStudentGroupQueryService _queryService;
    private readonly ICurrentUserService _currentUser;

    public GetMyPendingJoinRequestQueryHandler(
        IStudentGroupQueryService queryService,
        ICurrentUserService currentUser)
    {
        _queryService = queryService;
        _currentUser = currentUser;
    }

    public async Task<PendingJoinRequestDto?> Handle(
        GetMyPendingJoinRequestQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        return await _queryService.GetStudentPendingJoinRequestAsync(
            _currentUser.UserId.Value,
            request.SemesterId,
            cancellationToken);
    }
}
