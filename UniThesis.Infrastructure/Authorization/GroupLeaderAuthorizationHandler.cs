using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Infrastructure.Authorization.Requirements;
using AppClaimTypes = UniThesis.Application.Common.AppClaimTypes;

namespace UniThesis.Infrastructure.Authorization
{
    /// <summary>
    /// Handles group leader authorization.
    /// </summary>
    public class GroupLeaderAuthorizationHandler : AuthorizationHandler<GroupLeaderRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGroupRepository _groupRepository;

        public GroupLeaderAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            IGroupRepository groupRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _groupRepository = groupRepository;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GroupLeaderRequirement requirement)
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

            if (await _groupRepository.IsLeaderOfGroupAsync(userId.Value, groupId.Value))
            {
                context.Succeed(requirement);
            }
        }

        private Guid? GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(AppClaimTypes.DbUserId)?.Value;
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