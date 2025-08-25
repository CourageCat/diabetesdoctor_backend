


using NotificationService.Contract.Services.Conversation;
using NotificationService.Contract.Services.Conversation.Commands;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands.Conversations;


public class DeleteConversationCommandHandler(
    IConversationRepository groupRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteConversationCommand>
{
    public async Task<Result> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        var projection = Builders<Domain.Models.Conversation>.Projection.Include(group => group.Id);
        var group = await groupRepository.FindSingleAsync(group => group.ConversationId.Id == request.ConversationId,
            projection, cancellationToken);

        if (group is null)
        {
            throw new GroupException.GroupNotFoundException();
        }
        
        await groupRepository.DeleteAsync(unitOfWork.ClientSession, group.Id, cancellationToken);
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        
        return Result.Success();
    }
}