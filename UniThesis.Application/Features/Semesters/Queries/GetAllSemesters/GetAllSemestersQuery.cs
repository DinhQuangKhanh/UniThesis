using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Semesters.DTOs;

namespace UniThesis.Application.Features.Semesters.Queries.GetAllSemesters;

/// <summary>
/// Query to retrieve all semesters with their phases.
/// </summary>
public record GetAllSemestersQuery() : IQuery<List<SemesterDto>>;
