namespace AuthService.Api.Infrastructures.Abstractions.Services;

public interface IClaimsService
{
    public string? GetCurrentUserId { get; }
    public string? GetCurrentRole { get; }
}