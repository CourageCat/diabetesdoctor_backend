using ConsultationService.Contract.Services.ConsultationTemplate.Commands;
using FluentValidation;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Validators;

public class DeleteConsultationTemplateValidator : AbstractValidator<DeleteConsultationTemplatesCommand>
{
    public DeleteConsultationTemplateValidator()
    {
        RuleFor(x => x.TemplateIds)
            .Must(ids => ids.Any())
            .WithMessage("Danh sách khung giờ muốn xóa phải có tối thiểu 1 giá trị")
            .Must(ids => ids.All(id => !string.IsNullOrEmpty(id)))
            .WithMessage("Danh sách khung giờ muốn xóa không được chứa giá trị rỗng hoặc chỉ có khoảng trắng.")
            .Must(ids => ids.All(id => Guid.TryParse(id, out _)))
            .WithMessage("Phát hiện ID không phù hợp");
    }
}