namespace UserService.Domain.Models;

public class Media : DomainEntity<Guid>
{

    public string PublicId { get; private set; } = null!;
    public string PublicUrl { get; private set; } = null!;
    public Guid UploadedBy { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? ExpiredAt { get; private set; }
    public MediaType Type { get; private set; }
    
    public Guid? UserInfoId { get; private set; }
    public Guid? HospitalProfileId { get; private set; }
    public UserInfo? UserInfo { get; private set; } = default!;
    
    public HospitalProfile? HospitalProfile { get; private set; } = default!;
    

    public static Media Create(Guid id, string publicId, string publicUrl, MediaType type, Guid uploadedBy, Guid? userInfoId = null)
    {
        return new Media
        {
            Id = id,
            PublicId = publicId,
            PublicUrl = publicUrl,
            Type = type,
            UploadedBy = uploadedBy,
            IsUsed = false,
            ExpiredAt = DateTime.UtcNow.AddHours(1),
            UserInfoId = userInfoId ?? userInfoId,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
    
    public static Media CreateWithUsed(Guid id, string publicId, string publicUrl, MediaType type, Guid uploadedBy)
    {
        return new Media
        {
            Id = id,
            PublicId = publicId,
            PublicUrl = publicUrl,
            Type = type,
            UploadedBy = uploadedBy,
            IsUsed = true,
            ExpiredAt = null,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
    
    public static Media CreateForUserInfoForSeedData(Guid id, string publicId, string publicUrl, MediaType type, Guid uploadedBy, Guid userInfoId)
    {
        return new Media
        {
            Id = id,
            PublicId = publicId,
            PublicUrl = publicUrl,
            Type = type,
            UploadedBy = uploadedBy,
            IsUsed = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false,
            UserInfoId = userInfoId
        };
    }
    
    public static Media CreateForHospitalForSeedData(Guid id, string publicId, string publicUrl, MediaType type, Guid uploadedBy, Guid hospitalProfileId)
    {
        return new Media
        {
            Id = id,
            PublicId = publicId,
            PublicUrl = publicUrl,
            Type = type,
            UploadedBy = uploadedBy,
            IsUsed = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false,
            HospitalProfileId = hospitalProfileId
        };
    }

    public void Used()
    {
        IsUsed = true;
        ExpiredAt = null;
    }
    
    public void UnUsed()
    {
        IsUsed = false;
        ExpiredAt = DateTime.UtcNow.AddHours(1);
    }
}