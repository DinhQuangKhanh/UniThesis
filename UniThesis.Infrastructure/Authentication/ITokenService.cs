using UniThesis.Persistence.SqlServer.Identity;

namespace UniThesis.Infrastructure.Authentication;

public interface ITokenService
{
    Task<TokenResult> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles);
    Task<TokenResult?> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<bool> RevokeTokenAsync(Guid userId);
    TokenValidationResult ValidateToken(string token);
}