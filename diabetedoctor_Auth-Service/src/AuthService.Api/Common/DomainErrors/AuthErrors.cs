namespace AuthService.Api.Common.DomainErrors;

public static class AuthErrors
{
    public static readonly Error PhoneNumberAlreadyExists = Error.Conflict(AuthMessages.PhoneNumberAlreadyExists.GetMessage().Code,
        AuthMessages.PhoneNumberAlreadyExists.GetMessage().Message);
    public static readonly Error PhoneNumberNotFound = Error.Conflict(AuthMessages.PhoneNumberNotFound.GetMessage().Code,
        AuthMessages.PhoneNumberNotFound.GetMessage().Message);
    public static readonly Error PhoneNumberNotRegistered = Error.NotFound(AuthMessages.PhoneNumberNotRegistered.GetMessage().Code,
        AuthMessages.PhoneNumberNotRegistered.GetMessage().Message);
    public static readonly Error PasswordMismatch = Error.Conflict(AuthMessages.PasswordMismatch.GetMessage().Code,
        AuthMessages.PasswordMismatch.GetMessage().Message);
    public static readonly Error VerificationCodeSent = Error.Conflict(AuthMessages.VerificationCodeSent.GetMessage().Code,
        AuthMessages.VerificationCodeSent.GetMessage().Message);
    public static readonly Error VerificationCodeExpired = Error.Conflict(AuthMessages.VerificationCodeExpired.GetMessage().Code,
        AuthMessages.VerificationCodeExpired.GetMessage().Message);
    public static readonly Error UserBanned = Error.Conflict(AuthMessages.UserBanned.GetMessage().Code,
        AuthMessages.UserBanned.GetMessage().Message);
    public static readonly Error SessionExpired = Error.Conflict(AuthMessages.SessionExpired.GetMessage().Code,
        AuthMessages.SessionExpired.GetMessage().Message);
    public static readonly Error InvalidPhoneNumberOrPassword = Error.Conflict(AuthMessages.InvalidPhoneNumberOrPassword.GetMessage().Code,
        AuthMessages.InvalidPhoneNumberOrPassword.GetMessage().Message);
    public static readonly Error LoginTimeout = Error.Conflict(AuthMessages.LoginTimeout.GetMessage().Code,
        AuthMessages.LoginTimeout.GetMessage().Message);
    public static readonly Error InvalidOtpCode = Error.Conflict(AuthMessages.InvalidOtpCode.GetMessage().Code,
        AuthMessages.InvalidOtpCode.GetMessage().Message);
    public static readonly Error AccountExistException = Error.Conflict(AuthMessages.AccountExistException.GetMessage().Code,
        AuthMessages.AccountExistException.GetMessage().Message);
    public static readonly Error AccountNotExistException = Error.Conflict(AuthMessages.AccountNotExistException.GetMessage().Code,
        AuthMessages.AccountNotExistException.GetMessage().Message);
    public static readonly Error PasswordTooShort = Error.Conflict(AuthMessages.PasswordTooShort.GetMessage().Code,
        AuthMessages.PasswordTooShort.GetMessage().Message);
    public static readonly Error OtpCodeNotExist = Error.Conflict(AuthMessages.OtpCodeNotExist.GetMessage().Code,
        AuthMessages.OtpCodeNotExist.GetMessage().Message);
    public static readonly Error OtpCodeExpired = Error.Conflict(AuthMessages.OtpCodeExpired.GetMessage().Code,
        AuthMessages.OtpCodeExpired.GetMessage().Message);
    public static readonly Error EmailNotFound = Error.Conflict(AuthMessages.EmailNotFound.GetMessage().Code,
        AuthMessages.EmailNotFound.GetMessage().Message);
    public static readonly Error InvalidEmailOrPassword = Error.Conflict(AuthMessages.InvalidEmailOrPassword.GetMessage().Code,
        AuthMessages.InvalidEmailOrPassword.GetMessage().Message);
    public static readonly Error OldPasswordNotMatch = Error.Conflict(AuthMessages.OldPasswordNotMatch.GetMessage().Code,
        AuthMessages.OldPasswordNotMatch.GetMessage().Message);
}