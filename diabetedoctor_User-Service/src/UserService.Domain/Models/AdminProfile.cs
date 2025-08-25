namespace UserService.Domain.Models;

public sealed class AdminProfile : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public UserInfo User { get; private set; } = null!;

    private readonly List<HospitalAdmin> _hospitalAdmins = [];
    public IReadOnlyCollection<HospitalAdmin> HospitalAdmins => _hospitalAdmins.AsReadOnly();
    private readonly List<HospitalProfile> _hospitalProfiles = [];
    public IReadOnlyCollection<HospitalProfile> HospitalProfiles => _hospitalProfiles.AsReadOnly();
    private readonly List<ServicePackage> _servicePackages = [];
    public IReadOnlyCollection<ServicePackage> ServicePackages => _servicePackages.AsReadOnly();

    private AdminProfile(
        Guid id,
        Guid userId)
    {
        Id = id;
        UserId = userId;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    // Tạo thông tin
    public static AdminProfile Create
    (Guid id,
        Guid userId)
    {
        var adminProfile = new AdminProfile(id, userId);
        return adminProfile;
    }
}