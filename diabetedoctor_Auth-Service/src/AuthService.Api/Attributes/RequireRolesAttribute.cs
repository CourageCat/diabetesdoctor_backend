namespace AuthService.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireRolesAttribute : Attribute
{
    public string[] Roles { get; }
    public RequireRolesAttribute(params string[] roles)
    {
        Roles = roles;
    }
}
