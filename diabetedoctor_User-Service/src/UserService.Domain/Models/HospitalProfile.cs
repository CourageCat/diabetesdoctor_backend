namespace UserService.Domain.Models;

public sealed class HospitalProfile : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!; 
    public string Website { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string Introduction { get; private set; } = null!;
    public Image Thumbnail { get; private set; } = null!;
    public Guid AdminProfileId { get; private set; }
    public AdminProfile AdminProfile { get; private set; } = default!;
    
    private readonly List<Media> _medias = [];
    public IReadOnlyCollection<Media> Medias => _medias.AsReadOnly();
    
    private readonly List<DoctorProfile> _doctorProfiles = [];
    public IReadOnlyCollection<DoctorProfile> DoctorProfiles => _doctorProfiles.AsReadOnly();
    
    private readonly List<HospitalStaff> _hospitalStaffs = [];
    public IReadOnlyCollection<HospitalStaff> HospitalStaffs => _hospitalStaffs.AsReadOnly();

    private HospitalProfile()
    {
        
    }
    
    private HospitalProfile(
        Guid id,
        string name,
        string email,
        string phoneNumber,
        string website,
        string address,
        string introduction,
        Image thumbnail,
        Guid adminProfileId)
    {
        Id = id;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Website = website;
        Address = address;
        Introduction = introduction;
        Thumbnail = thumbnail;
        AdminProfileId = adminProfileId;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    // Tạo thông tin
    public static HospitalProfile Create
    (Guid id, 
        string name,
        string email,
        string phoneNumber,
        string website,
        string address,
        string introduction,
        Image thumbnail,
        Guid adminProfileId,
        IEnumerable<Media> medias)
    {
        var hospitalProfile =  new HospitalProfile(id, name, email, phoneNumber, website, address, introduction, thumbnail, adminProfileId);
        hospitalProfile._medias.AddRange(medias);
        //var domainEvent = HospitalProfileCreatedDomainEvent.Create(hospitalProfile);
        //hospitalProfile.AddDomainEvent(domainEvent);
        return hospitalProfile;
    }

    public void Update(string? name, string? email, string? phoneNumber, string? website, string? address,
        string? introduction, Image? thumbnail)
    {
        if (name != null)
        {
            Name = name;
        }
        if (email != null)
        {
            Email = email;
        }
        if (phoneNumber != null)
        {
            PhoneNumber = phoneNumber;
        }
        if (website != null)
        {
            Website = website;
        }
        if (address != null)
        {
            Address = address;
        }
        if (introduction != null)
        {
            Introduction = introduction;
        }

        if (thumbnail != null)
        {
            Thumbnail = thumbnail;
        }
    }
    
    public static HospitalProfile CreateForSeedData
    (Guid id, 
        string name,
        string email,
        string phoneNumber,
        string website,
        string address,
        string introduction,
        Image thumbnail,
        Guid adminProfileId)
    {
        var hospitalProfile =  new HospitalProfile(id, name, email, phoneNumber, website, address, introduction, thumbnail, adminProfileId);
        return hospitalProfile;
    }
}