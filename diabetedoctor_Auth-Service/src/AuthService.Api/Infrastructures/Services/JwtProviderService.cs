using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Api.Infrastructures.Services;

public sealed class JwtProviderService : IJwtProviderService
{
    private readonly AuthSettings _authSettings;

    public JwtProviderService(IOptions<AuthSettings> authSettings)
    {
        _authSettings = authSettings.Value;
    }

    public JwtTokenResult GenerateToken(Guid userId, List<string> roles)
    {
        var now = DateTime.UtcNow;

        var accessToken = GenerateJwtToken(
            userId: userId,
            roles: roles,
            secretKey: _authSettings.AccessSecretToken!,
            expires: now.AddMinutes(_authSettings.AccessTokenExpMinute)
        );

        var refreshToken = GenerateJwtToken(
            userId: userId,
            roles: roles,
            secretKey: _authSettings.RefreshSecretToken!,
            expires: now.AddMinutes(_authSettings.RefreshTokenExpMinute)
        );

        return new JwtTokenResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: now.AddMinutes(_authSettings.AccessTokenExpMinute)
        );
    }

    private string GenerateJwtToken(Guid userId, List<string> roles, string secretKey, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(AuthConstants.UserId, userId.ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _authSettings.Issuer,
            audience: _authSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateAndGetClaimRefreshToken(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = _authSettings.Issuer,
            ValidAudience = _authSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.RefreshSecretToken!))
        };

        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, parameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}