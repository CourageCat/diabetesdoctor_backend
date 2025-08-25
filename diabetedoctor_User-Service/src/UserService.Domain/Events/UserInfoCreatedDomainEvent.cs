using UserService.Contract.Helpers;

namespace UserService.Domain.Events;

public record UserInfoCreatedDomainEvent(Guid Id, UserInfo UserInfo, RoleType Role, Guid? HospitalId = null) : IDomainEvent
{
    public static UserInfoCreatedDomainEvent CreatePatient(UserInfo userInfo)
    {
        var id = new UuidV7().Value;
        return new UserInfoCreatedDomainEvent(id, userInfo, RoleType.Patient);
    }
    public static UserInfoCreatedDomainEvent CreateDoctor(UserInfo userInfo, Guid hospitalId)
    {
        var id = new UuidV7().Value;
        return new UserInfoCreatedDomainEvent(id, userInfo, RoleType.Doctor, hospitalId);
    }
    public static UserInfoCreatedDomainEvent CreateHospitalStaff(UserInfo userInfo, Guid hospitalId)
    {
        var id = new UuidV7().Value;
        return new UserInfoCreatedDomainEvent(id, userInfo, RoleType.HospitalStaff, hospitalId);
    }
    public static UserInfoCreatedDomainEvent CreateHospitalAdmin(UserInfo userInfo, Guid hospitalId)
    {
        var id = new UuidV7().Value;
        return new UserInfoCreatedDomainEvent(id, userInfo, RoleType.HospitalAdmin, hospitalId);
    }
    public static UserInfoCreatedDomainEvent CreateSystemAdmin(UserInfo userInfo)
    {
        var id = new UuidV7().Value;
        return new UserInfoCreatedDomainEvent(id, userInfo, RoleType.SystemAdmin);
    }
    public static UserInfoCreatedDomainEvent CreateModerator(UserInfo userInfo)
    {
        var id = new UuidV7().Value;
        return new UserInfoCreatedDomainEvent(id, userInfo, RoleType.Moderator);
    }
}