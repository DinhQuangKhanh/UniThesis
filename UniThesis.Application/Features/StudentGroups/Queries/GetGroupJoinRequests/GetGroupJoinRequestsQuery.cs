using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetGroupJoinRequests;

public record GetGroupJoinRequestsQuery(Guid GroupId) : IQuery<List<JoinRequestDto>>;
