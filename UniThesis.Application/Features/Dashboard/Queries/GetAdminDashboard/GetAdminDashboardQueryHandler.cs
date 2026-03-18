using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Dashboard.DTOs;

namespace UniThesis.Application.Features.Dashboard.Queries.GetAdminDashboard;

public class GetAdminDashboardQueryHandler : IQueryHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IAdminDashboardQueryService _queryService;

    public GetAdminDashboardQueryHandler(IAdminDashboardQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<AdminDashboardDto> Handle(
        GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        return await _queryService.GetDashboardAsync(cancellationToken);
    }
}
