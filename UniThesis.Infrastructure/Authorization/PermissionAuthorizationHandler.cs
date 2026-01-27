using Microsoft.AspNetCore.Authorization;
using UniThesis.Infrastructure.Authorization.Requirements;

namespace UniThesis.Infrastructure.Authorization
{
    /// <summary>
    /// Handles permission-based authorization.
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var permissions = context.User.FindAll("Permission").Select(c => c.Value);

            if (permissions.Contains(requirement.Permission) ||
                context.User.IsInRole("Admin")) // Admin has all permissions
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
