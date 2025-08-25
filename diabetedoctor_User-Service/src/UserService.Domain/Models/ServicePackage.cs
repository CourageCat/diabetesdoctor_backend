namespace UserService.Domain.Models;

public class ServicePackage : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public double Price { get; private set; }
    public int Sessions { get; private set; }
    public int DurationInMonths { get; private set; } 
    public bool IsActive { get; private set; }
    
    public Guid AdminProfileId { get; private set; }
    
    public AdminProfile AdminProfile { get; private set; } = null!;
    private readonly List<UserPackage> _userPackages = [];
    public IReadOnlyCollection<UserPackage> UserPackage => _userPackages.AsReadOnly();

    public ServicePackage()
    {
    }

    public ServicePackage(Guid id, string name, string description, double price, int sessions, int durationInMonths, Guid adminProfileId)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Sessions = sessions;
        DurationInMonths = durationInMonths;
        IsActive = true;
        AdminProfileId = adminProfileId;
    }

    public static ServicePackage Create(Guid id, string name, string description, double price, int sessions, int durationInMonths, Guid adminProfileId)
    {
        return new ServicePackage(id, name, description, price, sessions, durationInMonths, adminProfileId);
    }
}