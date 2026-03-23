using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetStudentGroup;

public record GetStudentGroupQuery(int? SemesterId) : IQuery<StudentGroupDto?>;
