using FluentValidation;

namespace MediaService.Contract.Services.Category.Validators;
public class GetTopPostCategoriesValidator : AbstractValidator<GetTopPostCategoriesQuery>
{
    public GetTopPostCategoriesValidator()
    {
        RuleFor(x => x.NumberOfCategories).NotEmpty().GreaterThan(0).WithMessage("Số lượng thể loại không được để trống và phải lớn hơn 0!");
    }
}
