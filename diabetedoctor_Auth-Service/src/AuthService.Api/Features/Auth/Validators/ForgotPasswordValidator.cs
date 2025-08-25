using AuthService.Api.Features.Auth.Commands;

namespace AuthService.Api.Features.Auth.Validators;

public class ForgotPasswordValidator :  AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage(AuthMessages.PhoneNumberNotFound.GetMessage().Message)
            .Matches(@"^\+?\d{9,15}$")
            .WithMessage(AuthMessages.InvalidPhoneNumberOrPassword.GetMessage().Message);
    }
}