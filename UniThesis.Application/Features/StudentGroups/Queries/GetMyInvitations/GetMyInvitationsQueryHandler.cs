using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetMyInvitations;

public class GetMyInvitationsQueryHandler : IQueryHandler<GetMyInvitationsQuery, List<InvitationDto>>
{
    private readonly IStudentGroupQueryService _queryService;
    private readonly ICurrentUserService _currentUser;

    public GetMyInvitationsQueryHandler(
        IStudentGroupQueryService queryService,
        ICurrentUserService currentUser)
    {
        _queryService = queryService;
        _currentUser = currentUser;
    }

    public async Task<List<InvitationDto>> Handle(
        GetMyInvitationsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        return await _queryService.GetStudentInvitationsAsync(
            _currentUser.UserId.Value, cancellationToken);
    }
}
