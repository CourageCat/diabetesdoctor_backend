namespace AuthService.Api.Persistences.Data.Models;

/// <summary>
/// Represents a user in the authentication system.
/// </summary>
public class User : AggregateRoot<Guid>
{
    public string Email { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public Image Avatar { get; private set; } = default!;
    public bool IsFirstUpdated { get; private set; } = default!;
    public List<AuthProvider> AuthProviders { get; private set; } = [];
    public List<UserRole> UserRoles { get; private set; } = [];
    public string? FcmToken { get; private set; }
    private User() { }

    /// <summary>
    /// Create a new User.
    /// </summary>
    public static User Create(Image avatar, string email, string fullName = "", string phoneNumber = "", Guid? id = null)
    {
        if (avatar == null)
            throw new ArgumentNullException("Không tìm thấy avatar mặc định");

        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Email = email,
            FullName = fullName,
            Avatar = avatar,
            PhoneNumber = phoneNumber,
            IsFirstUpdated = false
        };
    }

    /// <summary>
    /// Adds a phone-based login provider for the user.
    /// </summary>
    public void AddPhoneLogin(string phone, string passwordHash)
    {
        if (AuthProviders.Any(p => p.ProviderType == AuthProviderType.Phone))
            throw new InvalidOperationException("Đã có đăng nhập với số điện thoại");

        AuthProviders.Add(AuthProvider.CreatePhone(Id, phone, passwordHash));
    }
    
    /// <summary>
    /// Adds a email-based login provider for the user.
    /// </summary>
    public void AddEmailLogin(string email, string passwordHash)
    {
        if (AuthProviders.Any(p => p.ProviderType == AuthProviderType.Email))
            throw new InvalidOperationException("Đã có đăng nhập với email");

        AuthProviders.Add(AuthProvider.CreateEmail(Id, email, passwordHash));
    }

    /// <summary>
    /// Updates information of user
    /// </summary>
    public void UpdateInformation(string? fullName = null, Image? avatar = null, string? phoneNumber = null, string? email = null )
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            FullName = fullName;
        
        if (avatar is not null)
            Avatar = avatar;
        
        if (!string.IsNullOrWhiteSpace(phoneNumber))
            PhoneNumber = phoneNumber;
        
        if (!string.IsNullOrWhiteSpace(email))
            Email = email;
    }

    /// <summary>
    /// Assigns a role to the user if not already assigned.
    /// </summary>
    public void AssignRole(Guid roleId)
    {
        if (UserRoles.Any(r => r.RoleId == roleId))
            return;

        UserRoles.Add(UserRole.Assign(Id, roleId));
    }

    /// <summary>
    /// Checks if the user has a specific role.
    /// </summary>
    public bool HasRole(RoleType roleType)
    {
        return UserRoles.Any(r => r.Role.RoleType.Equals(roleType));
    }

    /// <summary>
    /// Marks the user as having completed their first update.
    /// </summary>
    public void MarkAsFirstUpdated() => IsFirstUpdated = true;

    public void UpdateFcmToken(string fcmToken)
    {
        FcmToken = fcmToken;
    }
}