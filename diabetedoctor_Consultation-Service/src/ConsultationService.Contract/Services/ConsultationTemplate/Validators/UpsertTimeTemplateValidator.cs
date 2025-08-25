using FluentValidation;
using MongoDB.Bson;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Validators;

public class UpsertTimeTemplateValidator : AbstractValidator<UpsertTimeTemplate>
{
    public UpsertTimeTemplateValidator()
    {
        RuleFor(x => x.TimeTemplateId)
            .NotEmpty().WithMessage("Id không được rỗng")
            .Must(id => !string.IsNullOrWhiteSpace(id) && ObjectId.TryParse(id, out _))
            .WithMessage("Id không hợp lệ")
            .When(x => x.Date is null);
        
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Ngày không được để trống")
            .When(x => string.IsNullOrWhiteSpace(x.TimeTemplateId));
        
        RuleFor(x => x.TimeRange)
            .Must(x => x.Start is not null && x.End is not null)
            .WithMessage("Thời gian không được để trống")
            .When(x => x.TimeTemplateId is null);
    }
}