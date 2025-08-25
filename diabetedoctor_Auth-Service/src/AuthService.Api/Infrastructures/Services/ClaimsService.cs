using System.Security.Claims;

namespace AuthService.Api.Infrastructures.Services;

public class ClaimsService : IClaimsService
{
    private readonly ClaimsPrincipal _user;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        //_user = httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string? GetCurrentUserId => _httpContextAccessor?.HttpContext?.Request.Headers["X-User-Id"].ToString();
    public string? GetCurrentRole => _httpContextAccessor?.HttpContext?.Request.Headers["X-User-Roles"].ToString();
    
    // public string GetCurrentUserId => _user.FindFirstValue("UserId") ?? string.Empty;
    // public string GetCurrentRole => _user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
