using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Mentor.DTOs;

namespace UniThesis.Application.Features.Mentor.Queries.GetMentorTopics;

public record GetMentorTopicsQuery(
    int? SemesterId,
    string? Search,
    int Page = 1,
    int PageSize = 10
) : IQuery<GetMentorTopicsResult>;
