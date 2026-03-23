using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.StudentGroups.DTOs;

namespace UniThesis.Application.Features.StudentGroups.Queries.GetMyInvitations;

public record GetMyInvitationsQuery : IQuery<List<InvitationDto>>;
