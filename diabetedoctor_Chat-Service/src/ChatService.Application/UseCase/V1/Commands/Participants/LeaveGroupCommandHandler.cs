using ChatService.Contract.Attributes;
using ChatService.Contract.Services.Participant.Commands;

namespace ChatService.Application.UseCase.V1.Commands.Participants;

public sealed class LeaveGroupCommandHandler(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    IParticipantRepository participantRepository,
    IConversationRepository conversationRepository) 
    : ICommandHandler<LeaveGroupCommand, Response>
{
    public async Task<Result<Response>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(request.UserId);
        var participant = await CheckParticipantExists(userId, request.ConversationId, cancellationToken);
        if (participant.IsFailure)
        {
            return Result.Failure<Response>(participant.Error);
        }

        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            await conversationRepository.RemoveMemberFromConversationAsync(unitOfWork.ClientSession, request.ConversationId, userId, cancellationToken);
            await participantRepository.SoftDeleteAsync(unitOfWork.ClientSession, participant.Value.Id , cancellationToken);
            var domainEvent = MapToDomainEvent(request);
            await publisher.Publish(domainEvent, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        
        return Result.Success(new Response(
            ConversationMessage.LeaveGroupConversationSuccessfully.GetMessage().Code,
            ConversationMessage.LeaveGroupConversationSuccessfully.GetMessage().Message));
    }

    private async Task<Result<Participant>> CheckParticipantExists(UserId userId, ObjectId conversationId, CancellationToken cancellationToken)
    {
        var projection = Builders<Participant>.Projection.Include(p => p.Id);
        var participant = await participantRepository.FindSingleAsync(
            u => u.UserId == userId && u.ConversationId == conversationId,
            projection,
            cancellationToken: cancellationToken );

        return participant is null ? Result.Failure<Participant>(ParticipantErrors.ParticipantNotFound) : Result.Success(participant);
    }
    
    private GroupMemberRemovedEvent MapToDomainEvent(LeaveGroupCommand command)
    {
        return new GroupMemberRemovedEvent(command.ConversationId.ToString(), command.UserId);
    }
}