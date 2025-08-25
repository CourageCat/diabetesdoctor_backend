using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class HospitalAdminErrors
{
    public static readonly Error HospitalAdminNotFound = Error.NotFound(HospitalAdminMessages.HospitalAdminNotFound.GetMessage().Code,
        HospitalAdminMessages.HospitalAdminNotFound.GetMessage().Message);
    public static readonly Error EmailAlreadyExists = Error.Conflict(HospitalAdminMessages.EmailAlreadyExists.GetMessage().Code,
        HospitalAdminMessages.EmailAlreadyExists.GetMessage().Message);
    public static readonly Error ProfileNotExist = Error.Conflict(HospitalAdminMessages.ProfileNotExist.GetMessage().Code,
        HospitalAdminMessages.ProfileNotExist.GetMessage().Message);
}