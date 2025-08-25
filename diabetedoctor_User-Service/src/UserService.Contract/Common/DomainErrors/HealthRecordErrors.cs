using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class HealthRecordErrors
{
    public static readonly Error HealthRecordNotFound = Error.NotFound(HealthRecordMessages.HealthRecordNotFound.GetMessage().Code,
        HealthRecordMessages.HealthRecordNotFound.GetMessage().Message);
    public static readonly Error HealthRecordNotBelongToUser = Error.Conflict(HealthRecordMessages.HealthRecordNotBelongToUser.GetMessage().Code,
        HealthRecordMessages.HealthRecordNotBelongToUser.GetMessage().Message);
    public static readonly Error GenerateAiNoteFailed = Error.Conflict(HealthRecordMessages.GenerateAiNoteFailed.GetMessage().Code,
        HealthRecordMessages.GenerateAiNoteFailed.GetMessage().Message);
}