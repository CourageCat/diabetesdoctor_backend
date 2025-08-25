using ChatService.Contract.Services.Conversation.IntegrationCommands;

namespace ChatService.Application.UseCase.V1.IntegrationCommands.Conversations.ConsultationConversation;

public sealed class ToggleGroupVisibilityCommandHandler(
    IUnitOfWork unitOfWork,
    IConversationRepository conversationRepository) 
    : ICommandHandler<ToggleGroupVisibilityCommand>
{
    public async Task<Result> Handle(ToggleGroupVisibilityCommand request, CancellationToken cancellationToken)
    {
        var conversationExisted = await conversationRepository.ExistsAsync(x => x.Id == request.ConversationId, cancellationToken);
        if (!conversationExisted)
        {
            return Result.Success();
        }
        
        var status = request.IsClosed ? ConversationStatus.Closed : ConversationStatus.Open;
        var update = Builders<Conversation>.Update.Set(x => x.Status, status);
        await conversationRepository.ToggleGroupVisibilityAsync(unitOfWork.ClientSession, request.ConversationId, update, cancellationToken);
        return Result.Success();
    }
}