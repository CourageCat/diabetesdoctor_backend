using ChatService.Contract.Services.Conversation.Commands.GroupConversation;
using ChatService.Contract.Services.Participant.Commands;

namespace ChatService.Contract.Services.Conversation.Validators;

public class AddMemberToGroupValidator : AbstractValidator<AddMembersToGroupCommand>
{
    public AddMemberToGroupValidator()
    {
        RuleFor(x => x.UserIds)
            .NotEmpty().WithMessage("Danh sách thành viên phải có tối thiểu 1 giá trị")
            .Must(members => members.All(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("Danh sách thành viên không được chứa giá trị rỗng hoặc chỉ có khoảng trắng.")
            .Must(members => members.All(id => Guid.TryParse(id, out _)))
            .WithMessage("Phát hiện ID không hợp lệ.");
    }
}