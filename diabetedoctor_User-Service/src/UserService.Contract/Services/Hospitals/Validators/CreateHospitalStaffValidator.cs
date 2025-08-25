using FluentValidation;
using UserService.Contract.Services.Hospitals.Commands;

namespace UserService.Contract.Services.Hospitals.Validators;

public class CreateHospitalStaffValidator : AbstractValidator<CreateHospitalStaffCommand>
{
    public CreateHospitalStaffValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Không được để trống TÊN");
            
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Không được để trống HỌ.");
            
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email nhân viên không được để trống")
            .EmailAddress()
            .WithMessage("Email phải đúng định dạng!");

        int minAge = 18;
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(dob => dob <= DateTime.Today.AddYears(-minAge))
            .WithMessage($"Nhân viên phải từ {minAge} tuổi trở lên!");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage("Giới tính không hợp lệ");

        RuleFor(x => x.AvatarId)
            .NotEmpty()
            .WithMessage("Ảnh đại diện không được để trống!");
    }
}