namespace AuthService.Api.Persistences.Data.Models;

public class AuthProvider : DomainEntity<Guid>
{
    public Guid UserId { get; private set; }
    public AuthProviderType ProviderType { get; private set; }
    public string ProviderKey { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public User User { get; private set; } = null!;
    private AuthProvider() { }
  
    public static AuthProvider CreatePhone(Guid userId, string phoneNumber, string passwordHash)
        => new()
        {
            UserId = userId,
            ProviderType = AuthProviderType.Phone,
            ProviderKey = phoneNumber,
            PasswordHash = passwordHash
        };
    
    public static AuthProvider CreateEmail(Guid userId, string email, string passwordHash)
        => new()
        {
            UserId = userId,
            ProviderType = AuthProviderType.Email,
            ProviderKey = email,
            PasswordHash = passwordHash
        };

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
}