using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UniThesis.Application.Common.Interfaces;

namespace UniThesis.Persistence.Services
{
    /// <summary>
    /// Implementation of ICurrentUserService using HttpContext.
    /// Provides access to current user information from the HTTP context.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public Guid? UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
            }
        }

        /// <inheritdoc />
        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        /// <inheritdoc />
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        /// <inheritdoc />
        public IEnumerable<string> Roles
        {
            get
            {
                var roles = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
                return roles?.Select(r => r.Value) ?? Enumerable.Empty<string>();
            }
        }

        /// <inheritdoc />
        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }
    }
}
