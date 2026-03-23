using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetMyPendingJoinRequest;

public record GetMyPendingJoinRequestQuery(int? SemesterId) : IQuery<PendingJoinRequestDto?>;
