using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.TopicPools.Commands.RejectRegistration;

/// <summary>
/// Command to reject a topic registration request (used by admin or department head).
/// </summary>
/// <param name="RegistrationId">The ID of the registration to reject.</param>
/// <param name="Reason">The reason for rejection.</param>
public record RejectTopicRegistrationCommand(
    Guid RegistrationId,
    string Reason) : ICommand;
