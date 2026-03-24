using UniThesis.Application.Features.Dashboard.DTOs;
using UniThesis.Application.Features.Departments.DTOs;

namespace UniThesis.Application.Common.Interfaces;

public interface IDepartmentHeadQueryService
{
    Task<DepartmentProjectsResponse> GetDepartmentProjectsAsync(
        int departmentId,
        CancellationToken cancellationToken = default);

    Task<List<DepartmentEvaluatorDto>> GetDepartmentEvaluatorsAsync(
        int departmentId,
        CancellationToken cancellationToken = default);

    Task<DepartmentHeadDashboardDto> GetDashboardAsync(
        int departmentId,
        Guid headId,
        CancellationToken cancellationToken = default);
}
