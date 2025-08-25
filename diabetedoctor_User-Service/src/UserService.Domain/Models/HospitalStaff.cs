namespace UserService.Domain.Models;

public sealed class HospitalStaff : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public Guid HospitalProfileId { get; private set; }
    public Guid HospitalAdminId { get; private set; }
    public UserInfo User { get; private set; } = null!;
    
    public HospitalProfile HospitalProfile { get; private set; } = default!;
    
    public HospitalAdmin HospitalAdmin { get; private set; } = default!;
    
    private readonly List<DoctorProfile> _doctorProfiles = [];
    public IReadOnlyCollection<DoctorProfile> DoctorProfiles => _doctorProfiles.AsReadOnly();
    
    private HospitalStaff(
        Guid id,
        Guid userId,
        Guid hospitalProfileId,
        Guid hospitalAdminId)
    {
        Id = id;
        UserId = userId;
        HospitalProfileId = hospitalProfileId;
        HospitalAdminId = hospitalAdminId;
        IsDeleted = false;
    }

    // Tạo thông tin
    public static HospitalStaff Create
    (Guid id, 
        Guid userId,
        Guid hospitalProfileId,
        Guid hospitalAdminId)
    {
        var hospitalStaff =  new HospitalStaff(id, userId, hospitalProfileId, hospitalAdminId);
        return hospitalStaff;
    }
}