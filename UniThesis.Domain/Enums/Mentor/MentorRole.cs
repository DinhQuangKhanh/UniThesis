namespace UniThesis.Domain.Enums.Mentor;

/// <summary>
/// Role of a mentor in a project.
/// </summary>
public enum MentorRole
{
    /// <summary>Primary mentor (required, only one per project).</summary>
    Primary = 0,

    /// <summary>Secondary/co-mentor (optional, maximum one per project).</summary>
    Secondary = 1
}