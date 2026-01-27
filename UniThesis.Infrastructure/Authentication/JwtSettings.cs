using System.Security.Claims;

namespace UniThesis.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "UniThesis";
    public string Audience { get; set; } = "UniThesis.Client";
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

public record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration,
    string TokenType = "Bearer"
);

public record TokenValidationResult(bool IsValid, ClaimsPrincipal? Principal, string? Error);
