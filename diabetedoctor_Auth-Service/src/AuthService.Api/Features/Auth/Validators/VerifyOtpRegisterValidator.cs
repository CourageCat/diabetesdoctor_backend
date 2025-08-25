using AuthService.Api.Features.Auth.Commands;

namespace AuthService.Api.Features.Auth.Validators;

public class VerifyOtpRegisterValidator
    : AbstractValidator<VerifyOtpRegisterCommand>
{
    public VerifyOtpRegisterValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
                .WithMessage(AuthMessages.PhoneNumberNotFound.GetMessage().Message)
            .Matches(@"^\+?\d{9,15}$")
                .WithMessage(AuthMessages.InvalidPhoneNumberOrPassword.GetMessage().Message);

        RuleFor(x => x.Otp)
            .NotEmpty()
                .WithMessage(AuthMessages.InvalidOtpCode.GetMessage().Message)
            .Length(6)
                .WithMessage(AuthMessages.InvalidOtpCode.GetMessage().Message);
    }
}