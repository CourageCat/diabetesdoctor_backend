using FluentValidation;
namespace MediaService.Contract.Services.FavouriteCategory.Validators;

public class UpdateFavouriteCategoryValidator : AbstractValidator<UpdateFavouriteCategoryCommand>
{      
    public UpdateFavouriteCategoryValidator()
    {
        RuleFor(x => x.CategoryIds)
        .NotNull()
        .Must(ids => ids != null
                 && ids.Count >= 3
                 && ids.Count <= 5
                 && ids.Distinct().Count() == ids.Count)
        .WithMessage("Các thể loại phải bao gồm từ 3 đến 5 giá trị không trùng!");

        RuleFor(x => x.UserId).NotEmpty().WithMessage("Id của người dùng không được để trống!");
    }
}
