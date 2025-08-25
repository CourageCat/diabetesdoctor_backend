
using NotificationService.Contract.Services.Conversation;
using NotificationService.Contract.Services.Conversation.Commands;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands.Conversations;

public class GroupMemberRemovedCommandHandler(
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<RemoveConversationMemberCommand>
{
    public async Task<Result> Handle(RemoveConversationMemberCommand request, CancellationToken cancellationToken)
    {
        var projection = Builders<Conversation>.Projection.Include(group => group.Id);
        var group = await conversationRepository.FindSingleAsync(g => g.ConversationId.Id == request.ConversationId, projection, cancellationToken);

        if (group is null)
        {
            throw new GroupException.GroupNotFoundException();
        }

        var update = Builders<Conversation>.Update.PullFilter(g => g.Members, m => m.Id == request.MemberId);
        
        await conversationRepository.UpdateOneAsync(unitOfWork.ClientSession, group.Id, update, cancellationToken: cancellationToken);
        return Result.Success();
    }
}