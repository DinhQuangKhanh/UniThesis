using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Departments.DTOs;

namespace UniThesis.Application.Features.Departments.Queries.GetDepartmentEvaluators;

public record GetDepartmentEvaluatorsQuery : IQuery<List<DepartmentEvaluatorDto>>;
