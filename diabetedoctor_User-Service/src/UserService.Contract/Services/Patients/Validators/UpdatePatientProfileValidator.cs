using FluentValidation;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Contract.Services.Patients.Validators;

public class UpdatePatientProfileValidator : AbstractValidator<UpdatePatientProfileCommand>
{
    public UpdatePatientProfileValidator()
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
    }
}