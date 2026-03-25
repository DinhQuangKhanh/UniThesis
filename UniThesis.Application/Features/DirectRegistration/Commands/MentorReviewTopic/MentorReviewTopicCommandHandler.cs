using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Application.Features.DirectRegistration.Commands.MentorReviewTopic;

public class MentorReviewTopicCommandHandler : ICommandHandler<MentorReviewTopicCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public MentorReviewTopicCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(MentorReviewTopicCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var project = await _projectRepository.GetWithMentorsAsync(request.ProjectId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Project), request.ProjectId);

        switch (request.Action.ToLowerInvariant())
        {
            case "approve":
                project.MentorApproveAndSubmit(userId);
                break;
            case "requestmodification":
                project.MentorRequestModification(request.Feedback);
                break;
            default:
                throw new BusinessRuleValidationException($"Hành động không hợp lệ: {request.Action}. Chỉ chấp nhận 'approve' hoặc 'requestModification'.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
