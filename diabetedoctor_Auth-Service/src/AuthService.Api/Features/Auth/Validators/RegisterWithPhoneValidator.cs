using AuthService.Api.Features.Auth.Commands;

namespace AuthService.Api.Features.Auth.Validators;

public sealed class RegisterWithPhoneValidator
    : AbstractValidator<RegisterWithPhoneCommand>
{
    public RegisterWithPhoneValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
                .WithMessage(AuthMessages.PhoneNumberNotFound.GetMessage().Message)
            .Matches(@"^\+?\d{9,15}$")
                .WithMessage(AuthMessages.InvalidPhoneNumberOrPassword.GetMessage().Message);

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage(AuthMessages.PasswordMismatch.GetMessage().Message)
            .MinimumLength(6)
                .WithMessage(AuthMessages.PasswordTooShort.GetMessage().Message);
    }
}