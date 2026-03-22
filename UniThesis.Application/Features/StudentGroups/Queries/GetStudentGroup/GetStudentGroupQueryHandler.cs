using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetStudentGroup;

public class GetStudentGroupQueryHandler : IQueryHandler<GetStudentGroupQuery, StudentGroupDto?>
{
    private readonly IStudentGroupQueryService _queryService;
    private readonly ICurrentUserService _currentUser;

    public GetStudentGroupQueryHandler(
        IStudentGroupQueryService queryService,
        ICurrentUserService currentUser)
    {
        _queryService = queryService;
        _currentUser = currentUser;
    }

    public async Task<StudentGroupDto?> Handle(
        GetStudentGroupQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        return await _queryService.GetStudentGroupAsync(
            _currentUser.UserId.Value, request.SemesterId, cancellationToken);
    }
}
