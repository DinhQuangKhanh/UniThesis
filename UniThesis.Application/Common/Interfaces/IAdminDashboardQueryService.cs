using UniThesis.Application.Features.Dashboard.DTOs;

namespace UniThesis.Application.Common.Interfaces;

public interface IAdminDashboardQueryService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}
