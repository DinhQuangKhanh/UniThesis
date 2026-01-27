using UniThesis.Domain.Aggregates.TopicPoolAggregate.Entities;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Specifications.TopicRegistrations;

/// <summary>
/// Specification to get registrations by status.
/// </summary>
public class RegistrationsByStatusSpec : BaseSpecification<TopicRegistration>
{
    /// <summary>
    /// Gets all registrations with a specific status.
    /// </summary>
    /// <param name="status">Registration status</param>
    public RegistrationsByStatusSpec(TopicRegistrationStatus status)
        : base(r => r.Status == status)
    {
        ApplyOrderByDescending(r => r.RegisteredAt);
    }
}
