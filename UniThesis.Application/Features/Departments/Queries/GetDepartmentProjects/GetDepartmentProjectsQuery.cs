using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Departments.DTOs;

namespace UniThesis.Application.Features.Departments.Queries.GetDepartmentProjects;

public record GetDepartmentProjectsQuery : IQuery<DepartmentProjectsResponse>;
