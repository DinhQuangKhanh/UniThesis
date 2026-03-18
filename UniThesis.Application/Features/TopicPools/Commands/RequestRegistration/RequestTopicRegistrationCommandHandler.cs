using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Services;

namespace UniThesis.Application.Features.TopicPools.Commands.RequestRegistration;

/// <summary>
/// Handles RequestTopicRegistrationCommand.
/// Delegates business logic to ITopicPoolDomainService and returns the new registration ID.
/// </summary>
public class RequestTopicRegistrationCommandHandler
    : ICommandHandler<RequestTopicRegistrationCommand, Guid>
{
    private readonly ITopicPoolDomainService _domainService;
    private readonly ICurrentUserService _currentUser;

    public RequestTopicRegistrationCommandHandler(
        ITopicPoolDomainService domainService,
        ICurrentUserService currentUser)
    {
        _domainService = domainService;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        RequestTopicRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var registration = await _domainService.RequestRegistrationAsync(
            projectId: request.ProjectId,
            groupId: request.GroupId,
            registeredBy: _currentUser.UserId.Value,
            note: request.Note,
            cancellationToken: cancellationToken);

        return registration.Id;
    }
}
