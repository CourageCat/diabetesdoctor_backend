using FluentValidation;
using Microsoft.AspNetCore.Http;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Contract.Services.Patients.Validators;

public class ChangeAvatarValidator : AbstractValidator<ChangeAvatarCommand>
{
    private readonly List<string> _allowedFileExtensions = new()
        { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".ppt", ".pptx", ".zip", ".zar" };

    public ChangeAvatarValidator()
    {
        RuleFor(x => x.Avatar)
            .NotEmpty().WithMessage("Tệp tin không được để trống!");

        RuleFor(x => x.Avatar)
            .Must(BeAValidFile)
            .WithMessage(
                "Tệp tin phải tuân theo các format (Ảnh: JPG, JPEG, PNG, GIF, BMP, Tệp tin: PDF, PPT, PPTX, ZIP, ZAR)!");
    }

    private bool BeAValidFile(IFormFile file)
    {
        if (file == null) return false;
        var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
        return _allowedFileExtensions.Contains(extension);
    }
}