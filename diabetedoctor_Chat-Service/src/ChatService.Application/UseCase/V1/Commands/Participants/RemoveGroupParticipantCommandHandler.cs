using ChatService.Contract.Attributes;
using ChatService.Contract.Services.Conversation.Commands.GroupConversation;

namespace ChatService.Application.UseCase.V1.Commands.Participants;

public sealed class RemoveGroupParticipantCommandHandler(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    IParticipantRepository participantRepository)
    : ICommandHandler<RemoveGroupParticipantCommand, Response>
{
    public async Task<Result<Response>> Handle(RemoveGroupParticipantCommand request, CancellationToken cancellationToken)
    {
        var user = await CheckStaffPermissionAsync(request, cancellationToken);
        if (user.IsFailure)
        {
            return Result.Failure<Response>(user.Error);
        }
        
        var participantCanBeRemoved = await EnsureMemberCanBeRemoved(request.ConversationId, request.MemberId, cancellationToken);
        if (participantCanBeRemoved.IsFailure)
        {
            return Result.Failure<Response>(participantCanBeRemoved.Error);
        }
        
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            await conversationRepository.RemoveMemberFromConversationAsync(unitOfWork.ClientSession, request.ConversationId, participantCanBeRemoved.Value.UserId, cancellationToken);
            await participantRepository.SoftDeleteAsync(unitOfWork.ClientSession, participantCanBeRemoved.Value.Id, cancellationToken);
            var domainEvent = MapToDomainEvent(request);
            await publisher.Publish(domainEvent, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }

        return Result.Success(new Response(
            ConversationMessage.RemoveMemberFromGroupSuccessfully.GetMessage().Code,
            ConversationMessage.RemoveMemberFromGroupSuccessfully.GetMessage().Message));
    }
    
    private async Task<Result> CheckStaffPermissionAsync(RemoveGroupParticipantCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(command.StaffId);
        var projection = Builders<User>.Projection.Include(u => u.HospitalId);
        var staff = await userRepository.FindSingleAsync(
            u => u.UserId == userId && u.IsDeleted == false && u.Role == Role.HospitalStaff && u.HospitalId != null,
            projection,
            cancellationToken: cancellationToken);

        if (staff is null)
        {
            return Result.Failure(UserErrors.StaffNotFound);
        }

        var isAllowed = await conversationRepository.ExistsAsync(
            c => c.Id == command.ConversationId 
                 && c.ConversationType == ConversationType.Group 
                 && c.HospitalId == staff.HospitalId,
            cancellationToken);

        return isAllowed
            ? Result.Success()
            : Result.Failure(ConversationErrors.NotFoundOrAccessDenied);
    }
    
    private async Task<Result<Participant>> EnsureMemberCanBeRemoved(ObjectId conversationId, string participantId, CancellationToken cancellationToken = default)
    {
        var userId = UserId.Of(participantId);
        var participant = await participantRepository.FindSingleAsync(
            p => p.UserId == userId && p.ConversationId == conversationId,
            cancellationToken: cancellationToken);

        if (participant is null)
        {
            return Result.Failure<Participant>(ParticipantErrors.ParticipantNotFound);
        }

        return participant.Role is MemberRole.Owner
            ? Result.Failure<Participant>(ConversationErrors.CannotRemoveMember)
            : Result.Success(participant);
    }
    
    private GroupMemberRemovedEvent MapToDomainEvent(RemoveGroupParticipantCommand command)
    {
        return new GroupMemberRemovedEvent(command.ConversationId.ToString(), command.MemberId);
    }
}