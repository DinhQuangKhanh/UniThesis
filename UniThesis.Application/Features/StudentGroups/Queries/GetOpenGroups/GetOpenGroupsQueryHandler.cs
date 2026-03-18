using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetOpenGroups;

public class GetOpenGroupsQueryHandler : IQueryHandler<GetOpenGroupsQuery, List<OpenGroupDto>>
{
    private readonly IStudentGroupQueryService _queryService;

    public GetOpenGroupsQueryHandler(IStudentGroupQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<List<OpenGroupDto>> Handle(
        GetOpenGroupsQuery request,
        CancellationToken cancellationToken)
    {
        return await _queryService.GetOpenGroupsAsync(request.SemesterId, cancellationToken);
    }
}
