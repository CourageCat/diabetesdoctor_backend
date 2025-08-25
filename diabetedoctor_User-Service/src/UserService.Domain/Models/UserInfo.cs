using UserService.Domain.Events;

namespace UserService.Domain.Models;

public class UserInfo : AggregateRoot<Guid>
{
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public Image Avatar { get; private set; } = null!;
    public FullName FullName { get; private set; }
    public string DisplayName { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public GenderType Gender { get; private set; }
    public RoleType Role { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsFirstUpdatedAvatar { get; private set; }
    public PatientProfile PatientProfile { get; private set; } = null!;
    public ModeratorProfile ModeratorProfile { get; private set; } = null!;
    public HospitalStaff HospitalStaff { get; private set; } = null!;
    public DoctorProfile DoctorProfile { get; private set; } = null!;
    public Wallet Wallet { get; private set; } = null!;
    public AdminProfile AdminProfile { get; private set; } = null!;
    private readonly List<PaymentHistory> _paymentHistories = [];
    public IReadOnlyCollection<PaymentHistory> PaymentHistory => _paymentHistories.AsReadOnly();
    private readonly List<UserPackage> _userPackages = [];
    public IReadOnlyCollection<UserPackage> UserPackage => _userPackages.AsReadOnly();
    private readonly List<Media> _medias = [];
    public IReadOnlyCollection<Media> Medias => _medias.AsReadOnly();

    private UserInfo()
    {
    }

    public UserInfo(Guid id, string? email, string? phoneNumber, Image avatar, FullName fullName, DateTime dateOfBirth,
        GenderType gender, bool isActive, RoleType role)
    {
        Id = id;
        Email = email;
        PhoneNumber = phoneNumber;
        Avatar = avatar;
        FullName = fullName;
        DisplayName = fullName.ToString();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Role = role;
        IsActive = isActive;
        IsFirstUpdatedAvatar = false;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static UserInfo CreatePatient(Guid id, string? email, string? phoneNumber, Image avatar, FullName fullName,
        DateTime dateOfBirth, GenderType gender, Media media)
    {
        var userInfo = new UserInfo(id, email, phoneNumber, avatar, fullName, dateOfBirth, gender, true,
            RoleType.Patient);
        userInfo._medias.Add(media);
        var domainEvent = UserInfoCreatedDomainEvent.CreatePatient(userInfo);
        userInfo.AddDomainEvent(domainEvent);
        return userInfo;
    }

    public void UpdateAvatar(Media avatar)
    {
        if (IsFirstUpdatedAvatar == false)
        {
            IsFirstUpdatedAvatar = true;
        }
        var imageUpdated = Image.Of(avatar.PublicId, avatar.PublicUrl);
        Avatar = imageUpdated;
        var domainEvent = UserInfoUpdatedDomainEvent.CreateUpdatedProfileEvent(Id, null, avatar.PublicUrl);
        AddDomainEvent(domainEvent);
    }

    public static UserInfo CreateDoctor(Guid id, string? email, string? phoneNumber, Image avatar, FullName fullName,
        DateTime dateOfBirth, GenderType gender, Media media, Guid hospitalId)
    {
        var userInfo = new UserInfo(id, email, phoneNumber, avatar, fullName, dateOfBirth, gender, true,
            RoleType.Doctor);
        userInfo._medias.Add(media);
        var domainEvent = UserInfoCreatedDomainEvent.CreateDoctor(userInfo, hospitalId);
        userInfo.AddDomainEvent(domainEvent);
        return userInfo;
    }

    public static UserInfo CreateHospitalStaff(Guid id, string? email, string? phoneNumber, Image avatar,
        FullName fullName, DateTime dateOfBirth, GenderType gender, Media media, Guid hospitalId)
    {
        var userInfo = new UserInfo(id, email, phoneNumber, avatar, fullName, dateOfBirth, gender, true,
            RoleType.HospitalStaff);
        userInfo._medias.Add(media);
        var domainEvent = UserInfoCreatedDomainEvent.CreateHospitalStaff(userInfo, hospitalId);
        userInfo.AddDomainEvent(domainEvent);
        return userInfo;
    }
    public static UserInfo CreateHospitalAdmin(Guid id, string? email, string? phoneNumber, Image avatar,
        FullName fullName, DateTime dateOfBirth, GenderType gender, Media media, Guid hospitalId)
    {
        var userInfo = new UserInfo(id, email, phoneNumber, avatar, fullName, dateOfBirth, gender, true,
            RoleType.HospitalAdmin);
        userInfo._medias.Add(media);
        var domainEvent = UserInfoCreatedDomainEvent.CreateHospitalAdmin(userInfo,  hospitalId);
        userInfo.AddDomainEvent(domainEvent);
        return userInfo;
    }
    public static UserInfo CreateSystemAdmin(Guid id, string? email, string? phoneNumber, Image avatar,
        FullName fullName, DateTime dateOfBirth, GenderType gender, Media media, Guid hospitalId)
    {
        var userInfo = new UserInfo(id, email, phoneNumber, avatar, fullName, dateOfBirth, gender, true,
            RoleType.SystemAdmin);
        userInfo._medias.Add(media);
        var domainEvent = UserInfoCreatedDomainEvent.CreateSystemAdmin(userInfo);
        userInfo.AddDomainEvent(domainEvent);
        return userInfo;
    }
    public static UserInfo CreateModerator(Guid id, string? email, string? phoneNumber, Image avatar,
        FullName fullName, DateTime dateOfBirth, GenderType gender, Media media, Guid hospitalId)
    {
        var userInfo = new UserInfo(id, email, phoneNumber, avatar, fullName, dateOfBirth, gender, true,
            RoleType.Moderator);
        userInfo._medias.Add(media);
        var domainEvent = UserInfoCreatedDomainEvent.CreateModerator(userInfo);
        userInfo.AddDomainEvent(domainEvent);
        return userInfo;
    }

    public static UserInfo CreateForSeedData(Guid id, string? email, string? phoneNumber, Image avatar,
        FullName fullName, DateTime dateOfBirth, GenderType gender, RoleType role)
    {
        var userInfo = new UserInfo(id, email, phoneNumber, avatar, fullName, dateOfBirth, gender, true, role);
        return userInfo;
    }

    // Cập nhật thông tin cơ bản
    public void UpdateBasicInfo(FullName? fullName, DateTime? dateOfBirth, GenderType? gender)
    {
        if (dateOfBirth is not null)
        {
            DateOfBirth = dateOfBirth.Value;
        }

        if (fullName is not null)
        {
            FullName = fullName;
            DisplayName = fullName.ToString();
        }

        if (dateOfBirth is not null)
        {
            DateOfBirth = dateOfBirth.Value;
        }

        if (gender is not null)
        {
            Gender = gender.Value;
        }

        var domainEvent = UserInfoUpdatedDomainEvent.CreateUpdatedProfileEvent(Id, fullName, null);
        AddDomainEvent(domainEvent);
    }
}