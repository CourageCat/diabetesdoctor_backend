using NotificationService.Contract.Services.Conversation.Commands;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands.Conversations;


public class AddMemberToGroupCommandHandler(
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<AddMemberToGroupCommand>
{
    public async Task<Result> Handle(AddMemberToGroupCommand request, CancellationToken cancellationToken)
    {
        var conversationId = ConversationId.Of(request.ConversationId);
        var conversationExisted = await conversationRepository.ExistAsync(c => c.ConversationId == conversationId, cancellationToken);

        if (!conversationExisted)
        {
            throw new GroupException.GroupNotFoundException();
        }
        
        var memberUserIds = request.Members.Select(UserId.Of).ToList();
        await conversationRepository.AddMemberToConversationAsync(unitOfWork.ClientSession, conversationId, memberUserIds, cancellationToken);
        return Result.Success();
    }
}