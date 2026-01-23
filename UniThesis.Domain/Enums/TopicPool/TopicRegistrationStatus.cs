namespace UniThesis.Domain.Enums.TopicPool;

/// <summary>
/// Status of a topic registration request.
/// </summary>
public enum TopicRegistrationStatus
{
    /// <summary>Registration is pending mentor confirmation.</summary>
    Pending = 0,

    /// <summary>Registration has been confirmed by the mentor.</summary>
    Confirmed = 1,

    /// <summary>Registration has been cancelled.</summary>
    Cancelled = 2
}