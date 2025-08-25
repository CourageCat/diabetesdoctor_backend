using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UserService.Contract.Infrastructure;

namespace UserService.Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly ClaimsPrincipal _user;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor accessor)
    {
        //_user = accessor.HttpContext?.User ?? throw new ArgumentNullException(nameof(accessor));
        _httpContextAccessor = accessor;
    }

    public string? UserId => _httpContextAccessor?.HttpContext?.Request.Headers["X-User-Id"].ToString();
    public string? Role => _httpContextAccessor?.HttpContext?.Request.Headers["X-User-Roles"].ToString();
    
    // public string? UserId => _user.FindFirstValue("UserId") ?? string.Empty;
    // public string? Role => _user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}