using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class DoctorErrors
{
    public static readonly Error PhoneNumberAlreadyExists = Error.Conflict(DoctorMessages.PhoneNumberAlreadyExists.GetMessage().Code,
        DoctorMessages.PhoneNumberAlreadyExists.GetMessage().Message);
    public static readonly Error DoctorNotFound = Error.NotFound(DoctorMessages.DoctorNotFound.GetMessage().Code,
        DoctorMessages.DoctorNotFound.GetMessage().Message);
    public static readonly Error ProfileNotExist = Error.NotFound(DoctorMessages.ProfileNotExist.GetMessage().Code,
        DoctorMessages.ProfileNotExist.GetMessage().Message);
    
}