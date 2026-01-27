using System.Security.Cryptography;
using UniThesis.Persistence.SqlServer.Identity;

namespace UniThesis.Infrastructure.Authentication
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
    
        public RefreshTokenService(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
    
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    
        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return false;
    
            return user.RefreshToken == refreshToken &&
                   user.RefreshTokenExpiryTime.HasValue &&
                   user.RefreshTokenExpiryTime.Value > DateTime.UtcNow;
        }
    
        public async Task SaveRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return;
            user.SetRefreshToken(refreshToken, expiryTime);
            await _userManager.UpdateAsync(user);
        }
    
        public async Task RevokeRefreshTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return;
    
            user.RevokeRefreshToken();
            await _userManager.UpdateAsync(user);
        }
    }
}
