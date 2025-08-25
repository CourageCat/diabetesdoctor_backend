using AuthService.Api.Features.Auth.Commands;

namespace AuthService.Api.Features.Auth.Validators;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage(AuthMessages.PasswordMismatch.GetMessage().Message)
            .MinimumLength(6)
            .WithMessage(AuthMessages.PasswordTooShort.GetMessage().Message);
        
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(AuthMessages.PasswordMismatch.GetMessage().Message)
            .MinimumLength(6)
            .WithMessage(AuthMessages.PasswordTooShort.GetMessage().Message);
    }
}