using System.Security.Claims;

namespace AuthService.Api.Infrastructures.Abstractions.Services;

public interface IJwtProviderService
{
    JwtTokenResult GenerateToken(Guid userId, List<string> roles);
    ClaimsPrincipal? ValidateAndGetClaimRefreshToken(string refreshToken);
}