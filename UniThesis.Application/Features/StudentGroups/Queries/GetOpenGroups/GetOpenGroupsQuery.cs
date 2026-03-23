using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetOpenGroups;

public record GetOpenGroupsQuery(int? SemesterId) : IQuery<List<OpenGroupDto>>;
