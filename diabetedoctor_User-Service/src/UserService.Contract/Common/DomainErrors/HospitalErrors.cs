using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class HospitalErrors
{
    public static readonly Error PhoneNumberAlreadyExists = Error.Conflict(HospitalMessages.PhoneNumberAlreadyExists.GetMessage().Code,
        HospitalMessages.PhoneNumberAlreadyExists.GetMessage().Message);
    public static readonly Error EmailAlreadyExists = Error.Conflict(HospitalMessages.EmailAlreadyExists.GetMessage().Code,
        HospitalMessages.EmailAlreadyExists.GetMessage().Message);
    public static readonly Error HospitalNotFound = Error.NotFound(HospitalMessages.HospitalNotFound.GetMessage().Code,
        HospitalMessages.HospitalNotFound.GetMessage().Message);
    
}