using FluentValidation;
using UserService.Contract.Services.ServicePackages.Commands;

namespace UserService.Contract.Services.ServicePackages.Validators;

public class CreateServicePackageValidator 
    : AbstractValidator<CreateServicePackageCommand>
{
    public CreateServicePackageValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Không được để trống tên của gói dịch vụ!");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Không được để trống mô tả của gói dịch vụ!");
        RuleFor(x => x.Price).NotNull().WithMessage("Không được để trống giá của gói dịch vụ!").Must(x => x > 0)
            .WithMessage("Giá tiền của gói dịch vụ phải lớn hơn 0");
        RuleFor(x => x.Sessions).NotNull().WithMessage("Không được để trống số lượt tư vấn!").Must(x => x > 0)
            .WithMessage("Số lượt tư vấn phải lớn hơn 0");
        RuleFor(x => x.DurationInMonths).NotNull().WithMessage("Không được để trống hạn sử dụng theo tháng!").Must(x => x > 0)
            .WithMessage("Hạn sử dụng theo tháng phải lớn hơn 0");
    }
}