using ChatService.Contract.Attributes;
using ChatService.Contract.Services.Participant.Commands;

namespace ChatService.Application.UseCase.V1.Commands.Participants;

public sealed class JoinGroupCommandHandler(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    IParticipantRepository participantRepository)
    : ICommandHandler<JoinGroupCommand, Response>
{
    public async Task<Result<Response>> Handle(JoinGroupCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await GetUserExistsAsync(request.UserId, cancellationToken);
        if (patientExists.IsFailure)
        {
            return Result.Failure<Response>(patientExists.Error);
        }
        var patient = patientExists.Value;
        
        var conversation = await GetConversationPermissionAsync(request.ConversationId, cancellationToken);
        if (conversation.IsFailure)
        {
            return Result.Failure<Response>(conversation.Error);
        }
        
        var dupResult = await CheckDuplicatedParticipantAsync(request.ConversationId, patient.UserId, cancellationToken);
        if (dupResult.IsFailure)
        {
            return Result.Failure<Response>(dupResult.Error);
        }
        
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            if (dupResult.Value is null)
            {
                var participant = MapToParticipant(request.ConversationId, patient, UserId.Of(request.InvitedBy));
                await participantRepository.CreateAsync(unitOfWork.ClientSession, participant, cancellationToken);
            }
            else
            {
                await participantRepository.RejoinToConversationAsync(unitOfWork.ClientSession, request.ConversationId, [patient.UserId], cancellationToken);
            }
            
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
            ConversationMessage.JoinGroupSuccessfully.GetMessage().Code,
            ConversationMessage.JoinGroupSuccessfully.GetMessage().Message));
    }
    
    private async Task<Result<User>> GetUserExistsAsync(string patientId, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(patientId);
        var user = await userRepository.FindSingleAsync(
            user => user.UserId ==  userId && user.IsDeleted == false,
            cancellationToken: cancellationToken);

        if (user is null)
        {
            return Result.Failure<User>(UserErrors.NotFound);
        }
        
        return user.Role is not Role.Patient
            ? Result.Failure<User>(UserErrors.MustHaveThisRole(nameof(Role.Patient)))
            : Result.Success(user);
    }
    
    private async Task<Result> GetConversationPermissionAsync(ObjectId conversationId,
        CancellationToken cancellationToken)
    {
        var isExisted = await conversationRepository.ExistsAsync(
            c => c.Id == conversationId 
                 && c.ConversationType == ConversationType.Group,
            cancellationToken: cancellationToken);

        return isExisted
            ? Result.Success()
            : Result.Failure(ConversationErrors.NotFound);
    }
    
    private async Task<Result<Participant?>> CheckDuplicatedParticipantAsync(ObjectId conversationId, UserId userId, CancellationToken cancellationToken)
    {
        var projection = Builders<Participant>.Projection
            .Include(p => p.UserId)
            .Include(p => p.ParticipantStatus)
            .Include(p => p.IsDeleted);
        
        var participant = await participantRepository.FindSingleAsync(
            p => p.UserId == userId && p.ConversationId == conversationId,
            projection,
            cancellationToken);

        if (participant is null)
        {
            return Result.Success(participant);
        }
        
        if (participant.ParticipantStatus is ParticipantStatus.LocalBan)
        {
            return Result.Failure<Participant?>(ParticipantErrors.YouAreBanned);
        }

        return participant.IsDeleted
            ? Result.Success<Participant?>(participant)
            : Result.Failure<Participant?>(ParticipantErrors.YouAlreadyInGroup);
    }
    
    private Participant MapToParticipant(ObjectId conversationId, User user, UserId invitedBy)
    {
        return Participant.CreateMember(
            id: ObjectId.GenerateNewId(),
            userId: user.UserId,
            conversationId: conversationId,
            fullName: user.FullName,
            avatar: user.Avatar,
            phoneNumber: user.PhoneNumber,
            email: user.Email,
            invitedBy: invitedBy);
    }
    
    private GroupMembersAddedEvent MapToDomainEvent(JoinGroupCommand command)
    {
        return new GroupMembersAddedEvent(command.ConversationId.ToString(), [command.UserId]);
    }    
}