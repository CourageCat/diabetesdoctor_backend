using NotificationService.Contract.Services.Conversation;
using NotificationService.Contract.Services.Conversation.Commands;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands.Conversations;

public class UpdateConversationCommandHandler(
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork) 
    : ICommandHandler<UpdateConversationCommand>
{
    public async Task<Result> Handle(UpdateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversationId = ConversationId.Of(request.ConversationId);
        var projection = Builders<Conversation>.Projection.Exclude(m => m.Members);
        var conversation = await conversationRepository.FindSingleAsync(g => g.ConversationId == conversationId, projection, cancellationToken: cancellationToken);

        if (conversation is null)
        {
            throw new GroupException.GroupNotFoundException();
        }
        
        var avatar = request.Avatar is not null ? Image.Of("avatar",request.Avatar) : null;
        await conversationRepository.UpdateConversationAsync(unitOfWork.ClientSession, conversationId, request.Name, avatar, cancellationToken: cancellationToken);

        return Result.Success();
    }
}