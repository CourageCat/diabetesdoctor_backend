using FluentValidation;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Contract.Services.Patients.Validators;

public class CreateCarePlanInstanceValidator : AbstractValidator<CreateCarePlanInstanceCommand>
{
    public CreateCarePlanInstanceValidator()
    {
        RuleFor(x => x.RecordType)
            .IsInEnum()
            .WithMessage("Thiếu loại chỉ số hoặc loại chỉ số không phù hợp!");
        RuleFor(x => x.SubType)
            .IsInEnum()
            .When(x => x.SubType != null)
            .WithMessage("Ngữ cảnh không phù hợp!");
        RuleFor(x => x.ScheduledAt)
            .Must(scheduleTime => scheduleTime >= DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Thời điểm đo phải lớn hơn hiện tại ít nhất 5 phút!");

    }
}