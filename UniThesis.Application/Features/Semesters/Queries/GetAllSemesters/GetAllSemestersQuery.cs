using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Semesters.DTOs;

namespace UniThesis.Application.Features.Semesters.Queries.GetAllSemesters;

/// <summary>
/// Query to retrieve all semesters with their phases, optionally filtered by status.
/// </summary>
public record GetAllSemestersQuery(string? Status = null) : IQuery<List<SemesterDto>>;
