using FluentValidation;
using UserService.Contract.Services.Doctors.Commands;

namespace UserService.Contract.Services.Doctors.Validators;

public class UpdateDoctorProfileValidator : AbstractValidator<UpdateDoctorProfileCommand>
{
    public UpdateDoctorProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .When(x => x.FirstName != null)
            .WithMessage("Không được để trống TÊN");
            
        RuleFor(x => x.LastName)
            .NotEmpty()
            .When(x => x.LastName != null)
            .WithMessage("Không được để trống HỌ.");

        int minAge = 18;
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(dob => dob <= DateTime.Today.AddYears(-minAge))
            .When(x => x.DateOfBirth != null)
            .WithMessage($"Bạn phải từ {minAge} tuổi trở lên!");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .When(x => x.Gender != null)
            .WithMessage("Giới tính không hợp lệ");
        
        RuleFor(x => x.NumberOfExperiences)
            .Must(x => x >= 0)
            .When(x => x.NumberOfExperiences != null)
            .WithMessage("Số năm kinh nghiệm không được nhỏ hơn 0!");
            
        RuleFor(x => x.Position)
            .IsInEnum()
            .When(x => x.Position != null)
            .WithMessage("Vị trí không hợp lệ!");

        RuleFor(x => x.Introduction)
            .NotEmpty()
            .When(x => x.Introduction != null)
            .WithMessage("Thông tin về bác sĩ không được để trống!");
    }
}