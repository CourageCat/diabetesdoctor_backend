using System.Globalization;
using FluentValidation;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Validators;

public class TimeTemplateValidator : AbstractValidator<TimeTemplate>
{
    public TimeTemplateValidator()
    {
        RuleFor(x => x.Times)
            .NotEmpty().WithMessage("Cần có ít nhất một khung giờ trong mỗi ngày.")
            .Must(x => x.All(time => time is not null))
            .WithMessage("Tồn tại khung giờ bị thiếu thông tin trong một số ngày.");
        
        RuleForEach(x => x.Times)
            .SetValidator(new TimeRangeValidator());
    }
}