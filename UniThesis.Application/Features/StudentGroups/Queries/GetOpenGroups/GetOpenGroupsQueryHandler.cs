using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetOpenGroups;

public class GetOpenGroupsQueryHandler : IQueryHandler<GetOpenGroupsQuery, List<OpenGroupDto>>
{
    private readonly IStudentGroupQueryService _queryService;
    private readonly ICurrentUserService _currentUserService;

    public GetOpenGroupsQueryHandler(
        IStudentGroupQueryService queryService,
        ICurrentUserService currentUserService)
    {
        _queryService = queryService;
        _currentUserService = currentUserService;
    }

    public async Task<List<OpenGroupDto>> Handle(
        GetOpenGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        return await _queryService.GetOpenGroupsAsync(studentId, request.SemesterId, cancellationToken);
    }
}
