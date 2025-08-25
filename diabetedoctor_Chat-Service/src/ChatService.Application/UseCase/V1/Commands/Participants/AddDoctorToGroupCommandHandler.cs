using ChatService.Contract.Attributes;
using ChatService.Contract.Services.Conversation.Commands.GroupConversation;
using ChatService.Contract.Services.Participant.Commands;

namespace ChatService.Application.UseCase.V1.Commands.Participants;

public sealed class AddDoctorToGroupCommandHandler(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    IParticipantRepository participantRepository)
    : ICommandHandler<AddDoctorToGroupCommand, Response>
{
    public async Task<Result<Response>> Handle(AddDoctorToGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await GetStaffPermissionAsync(request.StaffId, cancellationToken);
        if (user.IsFailure)
        {
            return Result.Failure<Response>(user.Error);
        }
        
        var conversation = await GetConversationPermissionAsync(request.ConversationId, user.Value, cancellationToken);
        if (conversation.IsFailure)
        {
            return Result.Failure<Response>(conversation.Error);
        }
        
        var doctorResult = await GetUserExistsAsync(request.DoctorId, user.Value, cancellationToken);
        if (doctorResult.IsFailure)
        {
            return Result.Failure<Response>(doctorResult.Error);
        }

        var dupResult = await CheckDuplicatedParticipantAsync(request.ConversationId, doctorResult.Value.UserId, cancellationToken);
        if (dupResult.IsFailure)
        {
            return Result.Failure<Response>(dupResult.Error);
        }
        
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            await conversationRepository.AddMemberToConversationAsync(unitOfWork.ClientSession, request.ConversationId, [doctorResult.Value.UserId], cancellationToken);
            if (dupResult.Value is null)
            {
                var participant = MapToParticipant(request.ConversationId, doctorResult.Value, user.Value.UserId);
                await participantRepository.CreateAsync(unitOfWork.ClientSession, participant, cancellationToken);
            }
            else
            {
                await participantRepository.RejoinToConversationAsync(unitOfWork.ClientSession, request.ConversationId, [doctorResult.Value.UserId], cancellationToken);
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
            ConversationMessage.AddDoctorToGroupSuccessfully.GetMessage().Code,
            ConversationMessage.AddDoctorToGroupSuccessfully.GetMessage().Message));
    }
    
    private async Task<Result<User>> GetStaffPermissionAsync(string staffId, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(staffId);
        var projection = Builders<User>.Projection.Include(u => u.HospitalId);
        var user = await userRepository.FindSingleAsync(
            u => u.UserId == userId && u.IsDeleted == false && u.Role == Role.HospitalStaff && u.HospitalId != null,
            projection,
            cancellationToken: cancellationToken);
        
        return user is not null
            ? Result.Success(user)
            : Result.Failure<User>(UserErrors.StaffNotFound);
    }
    
    private async Task<Result> GetConversationPermissionAsync(ObjectId conversationId, User staff,
        CancellationToken cancellationToken)
    {
        var isExisted = await conversationRepository.ExistsAsync(
            c => c.Id == conversationId 
                 && c.HospitalId == staff.HospitalId 
                 && c.ConversationType == ConversationType.Group,
            cancellationToken: cancellationToken);

        return isExisted
            ? Result.Success()
            : Result.Failure(ConversationErrors.NotFoundOrAccessDenied);
    }
    
    private async Task<Result<User>> GetUserExistsAsync(string doctorId, User staff, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(doctorId);
        var user = await userRepository.FindSingleAsync(
            u => u.UserId == userId && u.IsDeleted == false && u.Role == Role.Doctor,
            cancellationToken: cancellationToken);

        if (user is null)
        {
            return Result.Failure<User>(UserErrors.DoctorNotFound);
        }

        if (user.HospitalId is null || !user.HospitalId.Equals(staff.HospitalId))
        {
            return Result.Failure<User>(UserErrors.DoctorNotBelongToHospital);
        }
        
        return Result.Success(user);
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

        if (participant is null) return Result.Success(participant);
        
        if (participant.ParticipantStatus is ParticipantStatus.LocalBan) return Result.Failure<Participant?>(ParticipantErrors.ParticipantIsBanned);
        
        return participant.IsDeleted
            ? Result.Success<Participant?>(participant)
            : Result.Failure<Participant?>(ParticipantErrors.ParticipantAlreadyExisted);
    }
    
    private Participant MapToParticipant(ObjectId conversationId, User user, UserId invitedBy)
    {
        return Participant.CreateDoctor(
            id: ObjectId.GenerateNewId(),
            userId: user.UserId,
            conversationId: conversationId,
            fullName: user.FullName,
            avatar: user.Avatar,
            phoneNumber: user.PhoneNumber,
            email: user.Email,
            invitedBy: invitedBy);
    }
    
    private GroupMembersAddedEvent MapToDomainEvent(AddDoctorToGroupCommand command)
    {
        return new GroupMembersAddedEvent(command.ConversationId.ToString(), [command.DoctorId]);
    }    
}