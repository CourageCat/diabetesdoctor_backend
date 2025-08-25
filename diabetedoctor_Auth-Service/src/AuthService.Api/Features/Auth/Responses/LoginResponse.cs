namespace AuthService.Api.Features.Auth.Responses;

public record LoginResponse(JwtTokenResult AuthToken, AuthUserDto AuthUser);