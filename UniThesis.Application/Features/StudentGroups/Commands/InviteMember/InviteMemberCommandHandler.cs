using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;

namespace UniThesis.Application.Features.StudentGroups.Commands.InviteMember;

public class InviteMemberCommandHandler : ICommandHandler<InviteMemberCommand, int>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public InviteMemberCommandHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        var inviterId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var group = await _groupRepository.GetWithInvitationsAsync(request.GroupId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Group), request.GroupId);

        // Find the student by code
        var invitee = await _userRepository.GetByStudentCodeAsync(request.StudentCode, cancellationToken)
            ?? throw new EntityNotFoundException("User", request.StudentCode);

        // Check if invitee is already in an active group this semester
        if (await _groupRepository.IsStudentInActiveGroupAsync(invitee.Id, group.SemesterId, cancellationToken))
            throw new BusinessRuleValidationException("Student is already in an active group this semester.");

        // Domain logic validates leader, capacity, duplicates
        var invitation = group.InviteMember(inviterId, invitee.Id, request.Message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return invitation.Id;
    }
}
