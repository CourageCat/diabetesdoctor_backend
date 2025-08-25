using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MediaService.Contract.Services.Media.Validators
{
    public class UploadFilesValidator : AbstractValidator<UploadFilesCommand>
    {
        private readonly List<string> _allowedFileExtensions = new() { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".ppt", ".pptx", ".zip", ".zar" };

        public UploadFilesValidator()
        {
            RuleFor(x => x.Images)
                .NotEmpty().WithMessage("Tệp tin không được để trống!");

            RuleForEach(x => x.Images)
                .Must(BeAValidFile)
                .WithMessage("Tệp tin phải tuân theo các format (Ảnh: JPG, JPEG, PNG, GIF, BMP, Tệp tin: PDF, PPT, PPTX, ZIP, ZAR)!");
        }

        private bool BeAValidFile(IFormFile file)
        {
            if (file == null) return false;
            var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            return _allowedFileExtensions.Contains(extension);
        }
    }
}
