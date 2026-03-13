using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.TopicPools.Commands.ConfirmRegistration;

/// <summary>
/// Command to confirm a topic registration request (used by admin or department head).
/// </summary>
/// <param name="RegistrationId">The ID of the registration to confirm.</param>
public record ConfirmTopicRegistrationCommand(Guid RegistrationId) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate => ["topic-pools:"];
}
