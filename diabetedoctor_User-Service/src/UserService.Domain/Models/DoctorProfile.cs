namespace UserService.Domain.Models;

public sealed class DoctorProfile : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public int NumberOfExperiences { get; private set; }
    public DoctorPositionType Position { get; private set; }
    public string Introduction { get; private set; } = null!;
    public double Rating { get; private set; } = 0;
    public int NumberOfRating  { get; private set; }
    
    public Guid HospitalProfileId { get; private set; }
    public Guid HospitalStaffId { get; private set; }
    public UserInfo User { get; private set; } = null!;
    public HospitalProfile HospitalProfile { get; private set; } = default!;
    public HospitalStaff HospitalStaff { get; private set; } = default!;
    
    private readonly List<CarePlanMeasurementTemplate> _carePlanMeasurementTemplates = [];
    public IReadOnlyCollection<CarePlanMeasurementTemplate> CarePlanMeasurementTemplates => _carePlanMeasurementTemplates.AsReadOnly();

    private DoctorProfile()
    {
        
    }
    
    private DoctorProfile(
        Guid id,
        Guid userId,
        Guid hospitalProfileId,
        Guid hospitalStaffId,
        int numberOfExperiences,
        DoctorPositionType position,
        string introduction)
    {
        Id = id;
        UserId = userId;
        NumberOfExperiences = numberOfExperiences;
        Position = position;
        Introduction = introduction;
        Rating = 0;
        NumberOfRating = 0;
        HospitalProfileId = hospitalProfileId;
        HospitalStaffId = hospitalStaffId;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static DoctorProfile Create
    (Guid id, 
        Guid userId,
        Guid hospitalProfileId,
        Guid hospitalStaffId,
        int numberOfExperiences,
        DoctorPositionType position,
        string introduction)
    {
        var doctorProfile =  new DoctorProfile(id, userId, hospitalProfileId, hospitalStaffId, numberOfExperiences, position, introduction);
        return doctorProfile;
    }
    public static DoctorProfile CreateForSeedData
    (Guid id, 
        Guid userId,
        Guid hospitalProfileId,
        Guid hospitalStaffId,
        int numberOfExperiences,
        DoctorPositionType position,
        string introduction)
    {
        var doctorProfile = new DoctorProfile(id, userId, hospitalProfileId, hospitalStaffId, numberOfExperiences, position, introduction);
        return doctorProfile;
    }
    
    // Cập nhật thông tin cơ bản
    public void UpdateBasicInfo(int? numberOfExperiences, DoctorPositionType? position, string? introduction)
    {
        if (numberOfExperiences is not null)
        {
            NumberOfExperiences = numberOfExperiences.Value;
        }
        
        if (position is not null)
        {
            Position = position.Value;
        }

        if (introduction is not null)
        {
            Introduction = introduction;
        }
    }
}