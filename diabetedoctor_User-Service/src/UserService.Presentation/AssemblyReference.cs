using System.Reflection;

namespace UserService.Presentation;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
