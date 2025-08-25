using NotificationService.Contract.Services.Notification;

namespace NotificationService.Application.UseCases.V1.Commands.Notifications.Validators;

public class CreateNotificationValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Matches("^[a-zA-Z0-9 ]*$")
            .WithMessage("Special characters are not allowed.").MaximumLength(50).WithMessage("Độ dài của tiêu đề không quá 50 kí tự.");
        RuleFor(x => x.Body).NotEmpty().MaximumLength(500).WithMessage("Độ dài của nội dung không quá 500 kí tự.");
        RuleFor(x => x.UserIds).Must(x => x.All(id => Guid.TryParse(id, out var _)))
            .WithMessage("Thông tin người dùng không hợp lệ").When(x => x.UserIds.Any());
    }
}