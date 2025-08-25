namespace UserService.Domain.Models;

public sealed class HospitalAdmin : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public Guid HospitalProfileId { get; private set; }
    public Guid AdminProfileId { get; private set; }
    public UserInfo User { get; private set; } = null!;
    
    public HospitalProfile HospitalProfile { get; private set; } = default!;
    
    public AdminProfile AdminProfile { get; private set; } = default!;
    
    private readonly List<HospitalStaff> _hospitalStaffs = [];
    public IReadOnlyCollection<HospitalStaff> HospitalStaffs => _hospitalStaffs.AsReadOnly();
    
    private HospitalAdmin(
        Guid id,
        Guid userId,
        Guid hospitalProfileId,
        Guid adminProfileId)
    {
        Id = id;
        UserId = userId;
        HospitalProfileId = hospitalProfileId;
        AdminProfileId = adminProfileId;
        IsDeleted = false;
    }

    // Tạo thông tin
    public static HospitalAdmin Create
    (Guid id, 
        Guid userId,
        Guid hospitalProfileId,
        Guid adminProfileId)
    {
        var hospitalAdmin =  new HospitalAdmin(id, userId, hospitalProfileId, adminProfileId);
        return hospitalAdmin;
    }
}