using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class HospitalStaffErrors
{
    public static readonly Error HospitalStaffNotFound = Error.NotFound(HospitalStaffMessages.HospitalStaffNotFound.GetMessage().Code,
        HospitalStaffMessages.HospitalStaffNotFound.GetMessage().Message);
    public static readonly Error EmailAlreadyExists = Error.Conflict(HospitalStaffMessages.EmailAlreadyExists.GetMessage().Code,
        HospitalStaffMessages.EmailAlreadyExists.GetMessage().Message);
    public static readonly Error ProfileNotExist = Error.Conflict(HospitalStaffMessages.ProfileNotExist.GetMessage().Code,
        HospitalStaffMessages.ProfileNotExist.GetMessage().Message);
}