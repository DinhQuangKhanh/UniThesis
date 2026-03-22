using UniThesis.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;

namespace UniThesis.Application.Features.StudentGroups.Commands.RequestJoin;

public class RequestJoinCommandHandler : ICommandHandler<RequestJoinCommand, int>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public RequestJoinCommandHandler(
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(RequestJoinCommand request, CancellationToken cancellationToken)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var group = await _groupRepository.GetWithJoinRequestsAsync(request.GroupId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Group), request.GroupId);

        // Check if student is already in an active group this semester
        if (await _groupRepository.IsStudentInActiveGroupAsync(studentId, group.SemesterId, cancellationToken))
            throw new BusinessRuleValidationException("Student is already in an active group this semester.");

        // Domain logic validates group status, open for requests, capacity, duplicates
        var joinRequest = group.RequestToJoin(studentId, request.Message);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsPendingJoinRequestUniqueViolation(ex))
        {
            throw new BusinessRuleValidationException("A pending join request already exists for this student.");
        }

        return joinRequest.Id;
    }

    private static bool IsPendingJoinRequestUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException?.Message.Contains("IX_GroupJoinRequests_GroupId_StudentId_Pending", StringComparison.OrdinalIgnoreCase) == true
            || ex.Message.Contains("IX_GroupJoinRequests_GroupId_StudentId_Pending", StringComparison.OrdinalIgnoreCase);
    }
}
