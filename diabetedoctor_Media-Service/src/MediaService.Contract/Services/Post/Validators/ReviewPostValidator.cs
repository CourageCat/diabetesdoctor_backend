using FluentValidation;

namespace MediaService.Contract.Services.Post.Validators;
public class ReviewPostValidator : AbstractValidator<ReviewPostCommand>
{
    public ReviewPostValidator()
    {
        RuleFor(x => x.IsApproved).NotNull().WithMessage("Hãy xác định đồng ý hay từ chối bài viết này!"); ;
        RuleFor(x => x.ReasonRejected)
            .NotEmpty()
            .WithMessage("Lý do từ chối không được để trống!")
            .When(x => x.IsApproved == false);
    }
}
