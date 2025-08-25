namespace AuthService.Api.Persistences.Data.Models;

public class Role : DomainEntity<Guid>
{
    public RoleType RoleType { get; private set; } = default!;
    public List<UserRole> UserRoles { get; private set; } = [];
    private Role() { }

    public static Role Create(RoleType roleType)
    {
        return new Role { RoleType = roleType };
    }
}
