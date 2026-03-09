using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Semesters.DTOs;
using UniThesis.Domain.Aggregates.SemesterAggregate;

namespace UniThesis.Application.Features.Semesters.Queries.GetAllSemesters;

/// <summary>
/// Handles GetAllSemestersQuery by fetching all semesters and mapping to DTOs.
/// </summary>
public class GetAllSemestersQueryHandler : IQueryHandler<GetAllSemestersQuery, List<SemesterDto>>
{
    private readonly ISemesterRepository _semesterRepository;

    public GetAllSemestersQueryHandler(ISemesterRepository semesterRepository)
    {
        _semesterRepository = semesterRepository;
    }

    public async Task<List<SemesterDto>> Handle(GetAllSemestersQuery request, CancellationToken cancellationToken)
    {
        var semesters = await _semesterRepository.GetAllAsync(cancellationToken);

        return semesters.Select(s => new SemesterDto
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code.Value,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            Status = s.Status.ToString(),
            AcademicYear = s.AcademicYear.Value,
            Description = s.Description,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            Phases = s.Phases.OrderBy(p => p.Order).Select(p => new SemesterPhaseDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type.ToString(),
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Order = p.Order,
                Status = p.Status.ToString(),
                DurationDays = p.DurationDays
            }).ToList()
        }).ToList();
    }
}
