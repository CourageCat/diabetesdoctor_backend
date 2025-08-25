using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MediaService.Contract.Services.Media.Validators
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
