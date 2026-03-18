using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Semesters.DTOs;

namespace UniThesis.Application.Features.Semesters.Queries.GetSemesterById;

/// <summary>
/// Query to retrieve a single semester by its ID.
/// </summary>
public record GetSemesterByIdQuery(int SemesterId) : IQuery<SemesterDto>;
