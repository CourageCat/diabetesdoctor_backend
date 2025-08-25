using ChatService.Contract.Enums;
using ChatService.Contract.Services.User.Queries;

namespace ChatService.Contract.Services.User.Validators;

public class GetAvailableUsersForConversationValidator : AbstractValidator<GetAvailableUsersForConversationQuery>
{
    public GetAvailableUsersForConversationValidator()
    {
        RuleFor(x => x.Filters.Role)
            .Must(role => AllowedRoles.Contains(role))
            .WithMessage("Role bạn đang tìm không được cho phép");
    }
    
    private static readonly RoleEnum[] AllowedRoles =
    [
        RoleEnum.HospitalStaff,
        RoleEnum.Doctor,
        RoleEnum.Patient
    ];
}