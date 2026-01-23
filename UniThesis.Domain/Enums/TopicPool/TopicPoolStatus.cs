namespace UniThesis.Domain.Enums.TopicPool;

/// <summary>
/// Status of a topic in the topic pool.
/// </summary>
public enum TopicPoolStatus
{
    /// <summary>Topic is available for students to register.</summary>
    Available = 0,

    /// <summary>Topic has been selected by a student group.</summary>
    Selected = 1,

    /// <summary>Topic has expired (passed 2 semesters without selection).</summary>
    Expired = 2,

    /// <summary>Topic has been archived.</summary>
    Archived = 3
}