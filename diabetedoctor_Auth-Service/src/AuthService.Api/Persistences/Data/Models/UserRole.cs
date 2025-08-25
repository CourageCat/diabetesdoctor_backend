﻿namespace AuthService.Api.Persistences.Data.Models;

public class UserRole : DomainEntity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    private UserRole() { }

    public static UserRole Assign(Guid userId, Guid roleId)
    {
        return new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };
    }
}
