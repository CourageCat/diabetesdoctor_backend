using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class PatientErrors
{
    public static readonly Error ProfileExist = Error.Conflict(PatientMessages.ProfileExist.GetMessage().Code,
        PatientMessages.ProfileExist.GetMessage().Message);
    public static readonly Error ProfileNotExist = Error.NotFound(PatientMessages.ProfileNotExist.GetMessage().Code,
        PatientMessages.ProfileNotExist.GetMessage().Message);
    public static readonly Error HealthRecordExist = Error.Conflict(PatientMessages.HealthRecordExist.GetMessage().Code,
        PatientMessages.HealthRecordExist.GetMessage().Message);
    public static readonly Error PhoneNumberExist = Error.Conflict(PatientMessages.PhoneNumberExist.GetMessage().Code,
        PatientMessages.PhoneNumberExist.GetMessage().Message);
    public static readonly Error PhoneNumberNotRegistered = Error.Conflict(PatientMessages.PhoneNumberNotRegistered.GetMessage().Code,
        PatientMessages.PhoneNumberNotRegistered.GetMessage().Message);
    public static readonly Error ChangeAvatarFailed = Error.Conflict(PatientMessages.ChangeAvatarFailed.GetMessage().Code,
        PatientMessages.ChangeAvatarFailed.GetMessage().Message);
    public static readonly Error ConsultationSessionsNotFound = Error.Conflict(PatientMessages.ConsultationSessionsNotFound.GetMessage().Code,
        PatientMessages.ConsultationSessionsNotFound.GetMessage().Message);
    public static readonly Error ConsultationSessionsNotEnough = Error.Conflict(PatientMessages.ConsultationSessionsNotEnough.GetMessage().Code,
        PatientMessages.ConsultationSessionsNotEnough.GetMessage().Message);
}