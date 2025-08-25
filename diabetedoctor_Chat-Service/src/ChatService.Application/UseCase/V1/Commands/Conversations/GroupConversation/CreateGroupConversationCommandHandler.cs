using ChatService.Contract.Attributes;
using ChatService.Contract.Services.Conversation.Commands.GroupConversation;

namespace ChatService.Application.UseCase.V1.Commands.Conversations.GroupConversation;

public sealed class CreateGroupConversationCommandHandler(
    IUnitOfWork unitOfWork,
    IConversationRepository conversationRepository,
    IParticipantRepository participantRepository,
    IUserRepository userRepository,
    IMediaRepository mediaRepository,
    IPublisher publisher,
    IOptions<AppDefaultSettings> settings) 
    : ICommandHandler<CreateGroupConversationCommand, Response<CreateGroupConversationResponse>>
{
    public async Task<Result<Response<CreateGroupConversationResponse>>> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
    {
        var staff = await GetStaffPermissionAsync(request.OwnerId, cancellationToken);
        if (staff.IsFailure)
        {
            return Result.Failure<Response<CreateGroupConversationResponse>>(staff.Error);
        }
        
        Media? media = null;
        Image? avatar = null;
        if (request.AvatarId is not null)
        {
            var mediaId = ObjectId.Parse(request.AvatarId);
            media = await mediaRepository.FindByIdAsync(mediaId, cancellationToken);
            if (media is null)
            {
                return Result.Failure<Response<CreateGroupConversationResponse>>(MediaErrors.MediaNotFound);
            }
            avatar = Image.Of(media.PublicId, media.PublicUrl);
            media.Use();
        }
        
        var conversation = MapToConversation(request, avatar, staff.Value);
        var participant = MapToConversationParticipant(conversation.Id, staff.Value);
        
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            await conversationRepository.CreateAsync(unitOfWork.ClientSession, conversation, cancellationToken);
            if (media is not null)
            {
                await mediaRepository.ReplaceOneAsync(unitOfWork.ClientSession, media, cancellationToken);
            }
            await participantRepository.CreateAsync(unitOfWork.ClientSession, participant, cancellationToken);
            var domainEvent = MapToDomainEvent(conversation, [staff.Value.UserId.ToString()]);
            await publisher.Publish(domainEvent, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        
        return Result.Success(new Response<CreateGroupConversationResponse>(
            ConversationMessage.CreatedGroupSuccessfully.GetMessage().Code,
            ConversationMessage.CreatedGroupSuccessfully.GetMessage().Message, 
            new CreateGroupConversationResponse(conversation.Id.ToString())));
    }

    private async Task<Result<User>> GetStaffPermissionAsync(string ownerId, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(ownerId);
        var user = await userRepository.FindSingleAsync(
            u => u.UserId == userId && u.IsDeleted == false && u.Role == Role.HospitalStaff && u.HospitalId != null,
            cancellationToken: cancellationToken);

        return user is not null
            ? Result.Success(user)
            : Result.Failure<User>(UserErrors.StaffNotFound);
    }
    
    private Conversation MapToConversation(CreateGroupConversationCommand command, Image? avatar, User user)
    {
        var id = ObjectId.GenerateNewId();
        var defaultAvatar = Image.Of("default-avatar", settings.Value.GroupAvatarDefault);
        return Conversation.CreateGroup(id, command.Name, avatar ?? defaultAvatar, [user.UserId], user.HospitalId!);
    }
    
    private Participant MapToConversationParticipant(ObjectId conversationId, User owner)
    {
        return Participant.CreateOwner(
            id: ObjectId.GenerateNewId(),
            userId: owner.UserId,
            conversationId: conversationId,
            fullName: owner.FullName,
            avatar: owner.Avatar,
            phoneNumber: owner.PhoneNumber,
            email: owner.Email);
    }
    
    private ConversationCreatedEvent MapToDomainEvent(Conversation conversation, IEnumerable<string> memberIds)
    {
        return new ConversationCreatedEvent(conversation.Id.ToString(), conversation.Name, conversation.ConversationType, conversation.Avatar.ToString(), memberIds);
    }
}