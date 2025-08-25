using FluentValidation;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Contract.Services.Patients.Validators;

public class CreateCarePlanTemplateValidator : AbstractValidator<CreateCarePlanTemplateCommand>
{
    public CreateCarePlanTemplateValidator()
    {
        RuleFor(x => x.RecordType)
            .IsInEnum()
            .WithMessage("Thiếu loại chỉ số hoặc loại chỉ số không phù hợp!");
        RuleFor(x => x.SubType)
            .IsInEnum()
            .When(x => x.SubType != null)
            .WithMessage("Ngữ cảnh không phù hợp!");
    }
}