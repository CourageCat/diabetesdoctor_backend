using System.Globalization;
using ConsultationService.Contract.Services.ConsultationTemplate.Commands;
using FluentValidation;
using MongoDB.Bson;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Validators;

public class UpdateConsultationTemplateValidator : AbstractValidator<UpdateConsultationTemplateCommand>
{
    public UpdateConsultationTemplateValidator()
    {
        RuleForEach(x => x.UpsertTimeTemplates)
            .SetValidator(new UpsertTimeTemplateValidator());
        
        RuleForEach(x => x.TemplateIdsToDelete)
            .Must(id => !string.IsNullOrEmpty(id) && ObjectId.TryParse(id, out _))
            .WithMessage("Id để xóa không hợp lệ")
            .When(x => x.TemplateIdsToDelete.Any());
        
        // const string timeRegex = @"^(?:[01]\d|2[0-3]):[0-5]\d$";
        //
        // RuleFor(x => x.Start)
        //     .NotEmpty().WithMessage("Thời gian bắt đầu không được để trống.")
        //     .Matches(timeRegex).WithMessage("Thời gian bắt đầu sai định dạng.")
        //     .When(x => !string.IsNullOrEmpty(x.Start));
        //
        // RuleFor(x => x.End)
        //     .NotEmpty().WithMessage("Thời gian kết thúc không được để trống.")
        //     .Matches(timeRegex).WithMessage("Thời gian kết thúc sai định dạng.")
        //     .When(x => !string.IsNullOrEmpty(x.End));
        //
        // RuleFor(x => x)
        //     .Must(x => 
        //         TimeSpan.TryParseExact(x.Start, "hh\\:mm", CultureInfo.InvariantCulture, out var start) &&
        //         TimeSpan.TryParseExact(x.End, "hh\\:mm", CultureInfo.InvariantCulture, out var end) &&
        //         end > start)
        //     .WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.")
        //     .When(x => !string.IsNullOrEmpty(x.Start) && !string.IsNullOrEmpty(x.End));
    }
}