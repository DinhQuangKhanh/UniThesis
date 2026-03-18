using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Semesters.DTOs;

namespace UniThesis.Application.Features.Semesters.Queries.GetActiveSemester;

/// <summary>
/// Query to retrieve the currently active semester.
/// </summary>
public record GetActiveSemesterQuery() : IQuery<SemesterDto?>;
