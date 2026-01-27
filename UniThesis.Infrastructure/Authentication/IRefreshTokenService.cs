namespace UniThesis.Infrastructure.Authentication;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
    Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
    Task SaveRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime);
    Task RevokeRefreshTokenAsync(Guid userId);
}