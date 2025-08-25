using FluentValidation;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Contract.Services.Patients.Validators
{
    public class DeleteImagesValidator : AbstractValidator<DeleteFilesCommand>
    {

        public DeleteImagesValidator()
        {
            RuleFor(x => x.ImageIds)
                .NotEmpty().WithMessage("Id của những hình ảnh không được phép để trống!");
        }
    }
    
}
