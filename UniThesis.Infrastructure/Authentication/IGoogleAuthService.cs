namespace UniThesis.Infrastructure.Authentication;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken);
}

public record GoogleUserInfo(string Email, string Name, string? Picture, string GoogleId);