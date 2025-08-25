using FluentValidation;
using UserService.Contract.Services.Hospitals.Commands;

namespace UserService.Contract.Services.Hospitals.Validators;

public class CreateHospitalValidator : AbstractValidator<CreateHospitalCommand>
{
    public CreateHospitalValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên bệnh viện không được để trống!");
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email bệnh viện không được để trống")
            .EmailAddress()
            .WithMessage("Email phải đúng định dạng!");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Số điện thoại không được để trống!")
            .Matches(@"^(0\d{9}|\+84\d{9})$")
            .WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0 hoặc +84.");
        RuleFor(x => x.Website)
            .NotEmpty()
            .WithMessage("Đường dẫn đến website của bệnh viện không được để trống!");
        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Địa chỉ của bệnh viện không được để trống!");
        RuleFor(x => x.Introduction)
            .NotEmpty()
            .WithMessage("Thông tin về bệnh viện không được để trống!");
        RuleFor(x => x.Thumbnail)
            .NotNull()
            .WithMessage("Ảnh đại diện của bệnh viện không được để trống!");
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("Các hỉnh ảnh liên quan đến bệnh viện không được để trống!");
    }
}