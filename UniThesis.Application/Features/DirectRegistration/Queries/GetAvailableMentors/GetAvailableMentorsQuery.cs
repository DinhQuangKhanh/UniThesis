using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.DirectRegistration.Queries.GetAvailableMentors;

public record GetAvailableMentorsQuery(int? MajorId = null) : IQuery<List<AvailableMentorDto>>;
