namespace AuthService.Api.DTOs;

public record JwtTokenResult(string AccessToken, string? RefreshToken, DateTime ExpiresAt, string TokenType = AuthConstants.BearerTokenScheme);
