using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using ICurrentUserService = UniThesis.Application.Common.Interfaces.ICurrentUserService;

namespace UniThesis.Application.Features.Users.Commands.UnlockUser;

public class UnlockUserCommandHandler : ICommandHandler<UnlockUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthAccountService _authAccount;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UnlockUserCommandHandler(
        IUserRepository userRepository,
        IAuthAccountService authAccount,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _authAccount = authAccount;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(User), request.UserId);

        user.Activate();
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Re-enable auth account before committing DB changes
        await _authAccount.EnableAccountAsync(user.FirebaseUid, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
