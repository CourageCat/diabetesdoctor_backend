using FluentValidation;
using MediaService.Contract.Services.User;
using MongoDB.Driver;

namespace MediaService.Contract.Services.User.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {       
        public CreateUserValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id của người dùng không được để trống!");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Họ và tên của người dùng không được để trống!");
            RuleFor(x => x.PublicUrl).NotEmpty().WithMessage("Đường dẫn hình ảnh của người dùng không được để trống!");
        }      
    }
}
