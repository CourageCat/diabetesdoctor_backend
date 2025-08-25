using FluentValidation;

namespace MediaService.Contract.Services.Category.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Tên của thể loại không được để trống!");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Mô tả của thể loại không được để trống!");
    }
}
