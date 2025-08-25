
using NotificationService.Contract.Services.Conversation;
using NotificationService.Contract.Services.Conversation.Commands;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands.Conversations;


public class CreateConversationCommandHandler(IConversationRepository groupRepository, IUnitOfWork unitOfWork) :
    ICommandHandler<CreateConversationCommand>
{
    public async Task<Result> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var group = MapToGroup(request);
        await groupRepository.CreateAsync(unitOfWork.ClientSession, group, cancellationToken: cancellationToken);
        return Result.Success();
    }

    private Domain.Models.Conversation MapToGroup(CreateConversationCommand command)
    {
        var id = ObjectId.GenerateNewId();
        var groupId = ConversationId.Of(command.ConversationId);
        var avatar = Image.Of("avatar", command.Avatar);
        var memberIds = command.Members.Select(UserId.Of).ToList();
        return Domain.Models.Conversation.CreateGroup(id, groupId, command.ConversationName, avatar, memberIds);
    }
}