using Microsoft.AspNetCore.Authorization;

namespace UniThesis.Infrastructure.Authorization.Requirements
{
    /// <summary>
    /// Requirement for mentor of project authorization.
    /// </summary>
    public class MentorOfProjectRequirement : IAuthorizationRequirement { }
}
