using System.Globalization;
using FluentValidation;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Validators;

public class TimeRangeValidator : AbstractValidator<TimeRange>
{
    public TimeRangeValidator()
    {
        // const string timeRegex = @"^(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
        // var formats = new[] { "h\\:mm", "hh\\:mm" };
        
        RuleFor(x => x.Start)
            .NotEmpty().WithMessage("Thời gian bắt đầu không được để trống.");
        
        RuleFor(x => x.End)
            .NotEmpty().WithMessage("Thời gian kết thúc không được để trống.");
        
        RuleFor(x => x)
            .Must(x => x.End > x.Start)
            .WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
        
        // RuleFor(x => x)
        //     .Must(x => 
        //         TimeSpan.TryParseExact(x.Start, formats, CultureInfo.InvariantCulture, out var start) &&
        //         TimeSpan.TryParseExact(x.End, formats, CultureInfo.InvariantCulture, out var end) &&
        //         end > start)
        //     .WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
    }
}