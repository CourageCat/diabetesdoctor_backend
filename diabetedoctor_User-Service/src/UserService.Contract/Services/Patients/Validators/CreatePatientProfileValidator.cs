using FluentValidation;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Contract.Services.Patients.Validators;

public class CreatePatientProfileValidator : AbstractValidator<CreatePatientProfileCommand>
{
    public CreatePatientProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Không được để trống TÊN");
            
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Không được để trống HỌ.");

        int minAge = 18;
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(dob => dob <= DateTime.Today.AddYears(-minAge))
            .WithMessage($"Bạn phải từ {minAge} tuổi trở lên!");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage("Giới tính không hợp lệ");
    }
}