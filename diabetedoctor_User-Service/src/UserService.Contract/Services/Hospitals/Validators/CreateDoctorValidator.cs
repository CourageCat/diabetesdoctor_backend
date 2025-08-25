using FluentValidation;
using UserService.Contract.Services.Hospitals.Commands;

namespace UserService.Contract.Services.Hospitals.Validators
{
    public class CreateDoctorValidator : AbstractValidator<CreateDoctorCommand>
    {
        public CreateDoctorValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Không được để trống TÊN");
            
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Không được để trống HỌ.");
            
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Số điện thoại không được để trống!")
                .Matches(@"^(0\d{9}|\+84\d{9})$")
                .WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0 hoặc +84.");
            
            int minAge = 18;
            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .Must(dob => dob <= DateTime.Today.AddYears(-minAge))
                .WithMessage($"Bác sĩ phải từ {minAge} tuổi trở lên!");


            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage("Giới tính không hợp lệ");

            RuleFor(x => x.AvatarId)
                .NotEmpty()
                .WithMessage("Ảnh đại diện không được để trống!");

            RuleFor(x => x.NumberOfExperiences)
                .NotNull()
                .WithMessage("Số năm kinh nghiệm của bác sĩ không được để trống!")
                .Must(x => x >= 0)
                .WithMessage("Số năm kinh nghiệm không được nhỏ hơn 0!");
            
            RuleFor(x => x.Position)
                .IsInEnum()
                .WithMessage("Vị trí không hợp lệ!");

            RuleFor(x => x.Introduction)
                .NotEmpty()
                .WithMessage("Thông tin về bác sĩ không được để trống!");
        }
    }
}
