using FluentValidation;

namespace MediaService.Contract.Services.Post.Validators;
public class GetTopViewPostsValidator : AbstractValidator<GetTopViewPostsQuery>
{
    public GetTopViewPostsValidator()
    {
        RuleFor(x => x.NumberOfPosts).NotEmpty().GreaterThan(0).WithMessage("Số lượng bài viết không được để trống và phải lớn hơn 0!");
        RuleFor(x => x.NumberOfDays).NotEmpty().GreaterThan(0).WithMessage("Số lượng ngày không được để trống và phải lớn hơn 0!");
    }
}
