using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate.Rules;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Common.Exceptions;

namespace UniThesis.Application.Features.DirectRegistration.Queries.GetAvailableMentors;

public class GetAvailableMentorsQueryHandler : IQueryHandler<GetAvailableMentorsQuery, List<AvailableMentorDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ISemesterRepository _semesterRepository;

    public GetAvailableMentorsQueryHandler(
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        ISemesterRepository semesterRepository)
    {
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _semesterRepository = semesterRepository;
    }

    public async Task<List<AvailableMentorDto>> Handle(GetAvailableMentorsQuery request, CancellationToken cancellationToken)
    {
        // Get active semester → next semester
        var activeSemester = await _semesterRepository.GetActiveAsync(cancellationToken)
            ?? throw new BusinessRuleValidationException("Không tìm thấy học kỳ đang hoạt động.");

        var nextSemester = await _semesterRepository.GetSemesterAfterAsync(activeSemester.Id, 1, cancellationToken)
            ?? throw new BusinessRuleValidationException("Không tìm thấy học kỳ kế tiếp.");

        // Get all mentors
        var mentors = await _userRepository.GetByRoleAsync("Mentor", cancellationToken);

        var result = new List<AvailableMentorDto>();

        foreach (var mentor in mentors)
        {
            var groupCount = await _projectRepository.CountMentorActiveProjectsInSemesterAsync(
                mentor.Id, nextSemester.Id, cancellationToken);

            result.Add(new AvailableMentorDto(
                mentor.Id,
                mentor.FullName,
                mentor.Email.Value,
                mentor.AcademicTitle,
                groupCount,
                MentorCannotExceedMaxGroupsPerSemesterRule.MaxGroupsPerSemester));
        }

        return result.OrderBy(m => m.CurrentGroupCount).ThenBy(m => m.FullName).ToList();
    }
}
