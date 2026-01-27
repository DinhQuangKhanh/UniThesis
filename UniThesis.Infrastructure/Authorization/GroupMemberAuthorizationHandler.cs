using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Infrastructure.Authorization.Requirements;

namespace UniThesis.Infrastructure.Authorization
{
    /// <summary>
    /// Handles group member authorization.
    /// </summary>
    public class GroupMemberAuthorizationHandler : AuthorizationHandler<GroupMemberRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGroupRepository _groupRepository;

        public GroupMemberAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            IGroupRepository groupRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _groupRepository = groupRepository;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GroupMemberRequirement requirement)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            var userId = GetUserId(context.User);
            if (userId is null) return;

            var groupId = GetGroupIdFromRoute();
            if (groupId is null) return;

            var group = await _groupRepository.GetWithMembersAsync(groupId.Value);
            if (group is null) return;

            if (group.Members.Any(m => m.StudentId == userId && m.Status == Domain.Enums.Group.GroupMemberStatus.Active))
            {
                context.Succeed(requirement);
            }
        }

        private Guid? GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private Guid? GetGroupIdFromRoute()
        {
            var routeData = _httpContextAccessor.HttpContext?.GetRouteData();
            var groupIdStr = routeData?.Values["groupId"]?.ToString() ??
                            routeData?.Values["id"]?.ToString();
            return Guid.TryParse(groupIdStr, out var groupId) ? groupId : null;
        }
    }
}
