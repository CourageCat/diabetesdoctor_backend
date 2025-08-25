using ChatService.Contract.Services.Conversation.Commands.GroupConversation;

namespace ChatService.Contract.Services.Conversation.Validators;

public class CreateGroupConversationValidator : AbstractValidator<CreateGroupConversationCommand>
{
    public CreateGroupConversationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên nhóm không được để trống");
    }
}