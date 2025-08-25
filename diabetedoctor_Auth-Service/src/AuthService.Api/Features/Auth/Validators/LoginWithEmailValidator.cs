using AuthService.Api.Features.Auth.Commands;

namespace AuthService.Api.Features.Auth.Validators;

public sealed class LoginWithEmailValidator
    : AbstractValidator<LoginWithEmailCommand>
{
    public LoginWithEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(AuthMessages.EmailNotFound.GetMessage().Message)
            .EmailAddress()
            .WithMessage(AuthMessages.InvalidEmailOrPassword.GetMessage().Message);

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage(AuthMessages.PasswordMismatch.GetMessage().Message)
            .MinimumLength(6)
                .WithMessage(AuthMessages.PasswordTooShort.GetMessage().Message);
    }
}