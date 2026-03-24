using UniThesis.Application.Features.Dashboard.DTOs;

namespace UniThesis.Application.Common.Interfaces;

public interface IMentorDashboardQueryService
{
    Task<MentorDashboardDto> GetDashboardAsync(Guid mentorId, CancellationToken cancellationToken = default);
}
