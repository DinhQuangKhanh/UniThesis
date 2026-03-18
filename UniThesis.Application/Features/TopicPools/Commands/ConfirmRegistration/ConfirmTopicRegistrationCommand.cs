using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Attributes;

namespace UniThesis.Application.Features.TopicPools.Commands.ConfirmRegistration;

/// <summary>
/// Command to confirm a topic registration request (used by admin or department head).
/// </summary>
/// <param name="RegistrationId">The ID of the registration to confirm.</param>
[ActionLog("Confirm Topic Registration", "TopicPool")]
public record ConfirmTopicRegistrationCommand(Guid RegistrationId) : ICacheInvalidatingCommand
{
    public IReadOnlyCollection<string> CachePrefixesToInvalidate => ["topic-pools:"];
}
