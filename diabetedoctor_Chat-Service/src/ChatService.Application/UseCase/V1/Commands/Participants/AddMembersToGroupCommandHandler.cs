using ChatService.Contract.Attributes;
using ChatService.Contract.DTOs.UserDtos;
using ChatService.Contract.Services.Participant.Commands;

namespace ChatService.Application.UseCase.V1.Commands.Participants;

public sealed class AddMembersToGroupCommandHandler(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    IParticipantRepository participantRepository,
    IConversationRepository conversationRepository,
    IUserRepository userRepository)
    : ICommandHandler<AddMembersToGroupCommand, Response>
{
    public async Task<Result<Response>> Handle(AddMembersToGroupCommand request, CancellationToken cancellationToken)
    {
        var staff = await GetStaffPermissionAsync(request.StaffId, cancellationToken);
        if (staff.IsFailure)
        {
            return Result.Failure<Response>(staff.Error);
        }
        
        var conversation = await GetConversationPermissionAsync(request.ConversationId, staff.Value, cancellationToken);
        if (conversation.IsFailure)
        {
            return Result.Failure<Response>(conversation.Error);
        }
        
        var usersExists = await GetUsersExistsAsync(request.UserIds, cancellationToken);
        if (usersExists.IsFailure)
        {
            return Result.Failure<Response>(usersExists.Error);
        }

        var userIds = request.UserIds.Select(UserId.Of).ToList();
        var duplicatedParticipants = await CheckRejoinerOrDuplicatedParticipantsAsync(request.ConversationId, userIds, cancellationToken);
        if (duplicatedParticipants.IsFailure)
        {
            return Result.Failure<Response>(duplicatedParticipants.Error);
        }
        
        var allUserIds = usersExists.Value.Select(u => u.UserId).ToList();
        var rejoiners = duplicatedParticipants.Value;
        var newParticipants = rejoiners.Count > 0 
            ? usersExists.Value.Where(user => !rejoiners.Contains(user.UserId)).ToList()
            : usersExists.Value;
        
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            
            // add new participants
            await conversationRepository.AddMemberToConversationAsync(unitOfWork.ClientSession, request.ConversationId, allUserIds, cancellationToken);

            if (newParticipants.Count > 0)
            {
                var addParticipants = MapToListParticipant(request.ConversationId, newParticipants, staff.Value.UserId);
                await participantRepository.CreateManyAsync(unitOfWork.ClientSession, addParticipants, cancellationToken);
            }
            
            // rejoiners
            if (rejoiners.Count > 0)
            {
                await participantRepository.RejoinToConversationAsync(unitOfWork.ClientSession, request.ConversationId, rejoiners, cancellationToken);
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
            ConversationMessage.AddMemberToGroupSuccessfully.GetMessage().Code,
            ConversationMessage.AddMemberToGroupSuccessfully.GetMessage().Message));
    }
    
    private async Task<Result<User>> GetStaffPermissionAsync(string staffId, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(staffId);
        var projection = Builders<User>.Projection.Include(u => u.UserId).Include(u => u.HospitalId);
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
    
    private async Task<Result<List<User>>> GetUsersExistsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
    {
        var projection = Builders<User>.Projection
            .Include(user => user.UserId)
            .Include(user => user.FullName)
            .Include(user => user.Avatar)
            .Include(user => user.PhoneNumber)
            .Include(user => user.Email);
        
        var users = await userRepository.FindListAsync(
            user => userIds.Contains(user.UserId.Id)
                    && user.Role == Role.Patient
                    && user.IsDeleted == false,
            projection,
            cancellationToken);
        return users.Count == userIds.Count() ? Result.Success(users) : Result.Failure<List<User>>(UserErrors.NotFound);
    }
    
    private async Task<Result<List<UserId>>> CheckRejoinerOrDuplicatedParticipantsAsync(ObjectId conversationId, List<UserId> userIds, CancellationToken cancellationToken)
    {
        var participants = await participantRepository.FindListAsync(
            p => p.ConversationId == conversationId && userIds.Contains(p.UserId),
            cancellationToken: cancellationToken);
        
        var rejoiners = participants.Where(p => p is { IsDeleted: true, ParticipantStatus: ParticipantStatus.Active }).Select(p => p.UserId).ToList();
        var lookup = participants.Where(p => p is not { IsDeleted: true, ParticipantStatus: ParticipantStatus.Active }).ToLookup(p => p.ParticipantStatus is ParticipantStatus.LocalBan ? "banned" : "duplicated");
        var dupParticipants = lookup["duplicated"].ToList();
        var banParticipants = lookup["banned"].ToList();
        
        if (dupParticipants.Count == 0 && banParticipants.Count == 0) return Result.Success(rejoiners);
        
        var dupResult = new AddMemberToGroupResponse
        {
            MatchCount = participants.Count,
            DuplicatedUser = dupParticipants.Select(participant => new UserResponseDto
            {
                Id = participant.UserId.Id,
                FullName = participant.DisplayName,
                Avatar = participant.Avatar.ToString()
            }).ToList(),
            BannedUser = banParticipants.Select(participant => new UserResponseDto
            {
                Id = participant.UserId.Id,
                FullName = participant.DisplayName,
                Avatar = participant.Avatar.ToString()
            }).ToList()
        };
        return Result.Failure<List<UserId>>(ParticipantErrors.ParticipantAlreadyExistedOrBanned(dupResult));
    }
    
    private IEnumerable<Participant> MapToListParticipant(ObjectId conversationId, IEnumerable<User> users, UserId invitedBy)
    {
        var participants = users.Select(user =>
            Participant.CreateMember(
                id: ObjectId.GenerateNewId(),
                userId: user.UserId,
                conversationId: conversationId,
                fullName: user.FullName,
                avatar: user.Avatar,
                phoneNumber: user.PhoneNumber,
                email: user.Email,
                invitedBy: invitedBy)
        ).ToList();
        return participants;
    }
    
    private GroupMembersAddedEvent MapToDomainEvent(AddMembersToGroupCommand command)
    {
        return new GroupMembersAddedEvent(command.ConversationId.ToString(), command.UserIds);
    }
}