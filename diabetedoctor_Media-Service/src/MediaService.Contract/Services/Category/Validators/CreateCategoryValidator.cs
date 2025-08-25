using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MediaService.Contract.Services.Category.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Tên của thể loại không được để trống!");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Mô tả của thể loại không được để trống!");
            RuleFor(x => x.Image)
                .NotEmpty().WithMessage("Thiếu hình ảnh!")
                .Must(image => ObjectId.TryParse(image, out _))
                .WithMessage("Id của hình ảnh phải là ObjectId hợp lệ!");
        }
    }
}
