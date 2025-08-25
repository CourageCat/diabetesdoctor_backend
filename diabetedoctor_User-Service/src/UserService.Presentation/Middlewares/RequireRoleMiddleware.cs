using UserService.Contract.Attributes;

namespace UserService.Presentation.Middlewares;

public class RequireRoleMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var requireAuthAttr = endpoint?.Metadata.GetMetadata<RequireAuthenticatedAttribute>();
        var requireRolesAttr = endpoint?.Metadata.GetMetadata<RequireRolesAttribute>();

        if (requireAuthAttr != null)
        {
            var userId = context.Request.Headers["X-User-Id"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: missing or invalid user.");
                return;
            }
        }

        if (requireRolesAttr != null)
        {
            var header = context.Request.Headers["X-User-Roles"].ToString();
            var userRoles = header.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim());
            var required = requireRolesAttr.Roles;

            if (!userRoles.Any(r => required.Contains(r, StringComparer.OrdinalIgnoreCase)))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden: role not allowed.");
                return;
            }
        }

        await next(context);
    }
}
