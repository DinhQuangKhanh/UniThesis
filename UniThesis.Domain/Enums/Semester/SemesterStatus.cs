namespace UniThesis.Domain.Enums.Semester;

/// <summary>
/// Status of a semester.
/// </summary>
public enum SemesterStatus
{
    /// <summary>Semester is scheduled but not yet started.</summary>
    Upcoming = 0,

    /// <summary>Semester is currently active.</summary>
    Active = 1,

    /// <summary>Semester has ended.</summary>
    Closed = 2
}