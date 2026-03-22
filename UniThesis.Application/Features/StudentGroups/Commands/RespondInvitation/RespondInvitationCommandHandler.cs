using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;

namespace UniThesis.Application.Features.StudentGroups.Commands.RespondInvitation;

public class RespondInvitationCommandHandler : ICommandHandler<RespondInvitationCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public RespondInvitationCommandHandler(
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(RespondInvitationCommand request, CancellationToken cancellationToken)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var group = await _groupRepository.GetWithInvitationsAsync(request.GroupId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Group), request.GroupId);

        if (request.Accept)
        {
            // Check if student is already in another active group
            if (await _groupRepository.IsStudentInActiveGroupAsync(studentId, group.SemesterId, cancellationToken))
                throw new BusinessRuleValidationException("Student is already in an active group this semester.");

            group.AcceptInvitation(request.InvitationId, studentId);
        }
        else
        {
            group.RejectInvitation(request.InvitationId, studentId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
