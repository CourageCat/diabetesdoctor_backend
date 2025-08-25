using MediaService.Contract.Common.Constant;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MediaService.Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly ClaimsPrincipal _user;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor accessor)
    {
        _user = accessor.HttpContext?.User ?? new ClaimsPrincipal();
        _httpContextAccessor = accessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].ToString();
    public string? Role => _httpContextAccessor.HttpContext?.Request.Headers["X-User-Roles"].ToString();
}