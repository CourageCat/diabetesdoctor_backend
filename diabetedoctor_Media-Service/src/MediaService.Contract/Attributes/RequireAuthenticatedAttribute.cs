namespace MediaService.Contract.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireAuthenticatedAttribute : Attribute
{
}
