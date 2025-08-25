using System.Globalization;
using ConsultationService.Contract.Services.ConsultationTemplate.Commands;
using FluentValidation;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Validators;

public class CreateConsultationTemplatesValidator : AbstractValidator<CreateConsultationTemplatesCommand>
{
    public CreateConsultationTemplatesValidator()
    {
        RuleFor(x => x.TimeTemplates)
            .NotEmpty().WithMessage("Danh sách ngày và khung giờ không được để trống.")
            .Must(x => x.All(template => template is not null))
            .WithMessage("Tồn tại ngày bị thiếu thông tin khung giờ.");
        
        RuleForEach(x => x.TimeTemplates)
            .SetValidator(new TimeTemplateValidator());
    }
}

