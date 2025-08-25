namespace ConsultationService.Contract.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireRolesAttribute : Attribute
{
    public string[] Roles { get; }
    public RequireRolesAttribute(params string[] roles)
    {
        Roles = roles;
    }
}
