using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.GroupAggregate;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Application.Features.DirectRegistration.Commands.SubmitToMentor;

public class SubmitToMentorCommandHandler : ICommandHandler<SubmitToMentorCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public SubmitToMentorCommandHandler(
        IProjectRepository projectRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(SubmitToMentorCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var project = await _projectRepository.GetWithMentorsAsync(request.ProjectId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Project), request.ProjectId);

        // Validate: user must be leader of the group
        if (!project.GroupId.HasValue)
            throw new BusinessRuleValidationException("Đề tài chưa được gán cho nhóm nào.");

        // Submit based on current status
        if (project.Status == ProjectStatus.Draft)
            project.SubmitToMentor(userId);
        else if (project.Status == ProjectStatus.NeedsModification)
            project.ResubmitToMentor(userId);
        else
            throw new BusinessRuleValidationException("Đề tài không ở trạng thái có thể gửi cho giảng viên.");

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
