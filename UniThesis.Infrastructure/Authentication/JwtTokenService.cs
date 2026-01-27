using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UniThesis.Persistence.SqlServer.Identity;

namespace UniThesis.Infrastructure.Authentication
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly IRefreshTokenService _refreshTokenService;

        public JwtTokenService(IOptions<JwtSettings> settings, IRefreshTokenService refreshTokenService)
        {
            _settings = settings.Value;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<TokenResult> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("FullName", user.FullName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(user.StudentCode))
                claims.Add(new Claim("StudentCode", user.StudentCode));
            if (!string.IsNullOrEmpty(user.EmployeeCode))
                claims.Add(new Claim("EmployeeCode", user.EmployeeCode));
            if (user.DepartmentId.HasValue)
            claims.Add(new Claim("DepartmentId", user.DepartmentId.Value.ToString()));

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: accessTokenExpiration,
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = _refreshTokenService.GenerateRefreshToken();

            await _refreshTokenService.SaveRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiration);

            return new TokenResult(accessToken, refreshToken, accessTokenExpiration, refreshTokenExpiration);
        }

        public async Task<TokenResult?> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value is not { } userIdStr)
                return null;

            if (!Guid.TryParse(userIdStr, out var userId))
                return null;

            var isValid = await _refreshTokenService.ValidateRefreshTokenAsync(userId, refreshToken);
            if (!isValid)
                return null;

            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var newToken = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: principal.Claims.Where(c => c.Type != JwtRegisteredClaimNames.Jti)
                    .Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())),
                expires: accessTokenExpiration,
                signingCredentials: credentials
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(newToken);
            var newRefreshToken = _refreshTokenService.GenerateRefreshToken();

            await _refreshTokenService.SaveRefreshTokenAsync(userId, newRefreshToken, refreshTokenExpiration);

            return new TokenResult(newAccessToken, newRefreshToken, accessTokenExpiration, refreshTokenExpiration);
        }

        public async Task<bool> RevokeTokenAsync(Guid userId)
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(userId);
            return true;
        }

        public TokenValidationResult ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_settings.Secret);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return new TokenValidationResult(true, principal, null);
            }
            catch (Exception ex)
            {
                return new TokenValidationResult(false, null, ex.Message);
            }
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Secret);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _settings.Audience,
                    ValidateLifetime = false // Allow expired tokens
                }, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
