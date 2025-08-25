using FluentValidation;
using UserService.Contract.Services.Hospitals.Commands;

namespace UserService.Contract.Services.Hospitals.Validators;

public class UpdateHospitalValidator : AbstractValidator<UpdateHospitalCommand>
{
    public UpdateHospitalValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name != null)
            .WithMessage("Tên bệnh viện không được để trống!");
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email bệnh viện không được để trống")
            .EmailAddress()
            .WithMessage("Email phải đúng định dạng!")
            .When(x => x.Email != null);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Số điện thoại không được để trống!")
            .Matches(@"^(0\d{9}|\+84\d{9})$")
            .WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0 hoặc +84.")
            .When(x => x.PhoneNumber != null);
        RuleFor(x => x.Website)
            .NotEmpty()
            .WithMessage("Đường dẫn đến website của bệnh viện không được để trống!")
            .When(x => x.Website != null);
        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Địa chỉ của bệnh viện không được để trống!")
            .When(x => x.Address != null);
        RuleFor(x => x.Introduction)
            .NotEmpty()
            .WithMessage("Thông tin về bệnh viện không được để trống!")
            .When(x => x.Introduction != null);
        RuleFor(x => x.Thumbnail)
            .NotNull()
            .WithMessage("Ảnh đại diện của bệnh viện không được để trống!")
            .When(x => x.Thumbnail != null);
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("Các hỉnh ảnh liên quan đến bệnh viện không được để trống!")
            .When(x => x.Images != null);
    }
}