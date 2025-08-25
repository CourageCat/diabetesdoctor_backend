using ChatService.Contract.Services.Conversation.Commands.PersonalConversation;

namespace ChatService.Application.UseCase.V1.IntegrationCommands.Conversations.ConsultationConversation;

public sealed class CreateConsultationConversationCommandHandler(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    IOptions<AppDefaultSettings> appDefaultSettings,
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    IParticipantRepository participantRepository)
    : ICommandHandler<CreateConsultationConversationCommand>
{
    public async Task<Result> Handle(CreateConsultationConversationCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(request.PatientId);
        var doctorId = UserId.Of(request.DoctorId);
        var userIds = new List<UserId>{ userId, doctorId };
        
        var oldConversation = await conversationRepository.FindSingleAsync(
            c => c.Members.Count == userIds.Count 
                 && userIds.All(id => c.Members.Contains(id)) 
                 && c.ConversationType == ConversationType.Consultation,
            cancellationToken: cancellationToken);

        IDomainEvent domainEvent;
        if (oldConversation is not null)
        {
            if (request.IsOpened)
            {
                oldConversation.Open();
            }
            else
            {
                oldConversation.Close();
            }
            domainEvent = MapToConversationLinkedEvent(oldConversation, request.ConsultationId);
        }
        else
        {
            var users = await GetUsers(request, cancellationToken);
            var conversation = MapToConversation(users.Value, request.IsOpened);
            var participants = MapToParticipants(conversation.Id, users.Value);
            await conversationRepository.CreateAsync(unitOfWork.ClientSession, conversation, cancellationToken);
            await participantRepository.CreateManyAsync(unitOfWork.ClientSession, participants, cancellationToken);
            domainEvent = MapToConversationCreatedEvent(conversation, users.Value, request.ConsultationId);
        }
        
        await publisher.Publish(domainEvent, cancellationToken);
        return Result.Success();
    }

    private async Task<Result<HashSet<User>>> GetUsers(CreateConsultationConversationCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(command.PatientId);
        var doctorId = UserId.Of(command.DoctorId);
        var userIds = new List<UserId>{userId, doctorId};
        var users = (await userRepository.FindListAsync(u => userIds.Contains(u.UserId), cancellationToken: cancellationToken)).ToHashSet();
        return users.Count == 2 ? Result.Success(users) : throw new UserExceptions.UserNotFoundException();
    }
    
    private Conversation MapToConversation(HashSet<User> users, bool isOpened)
    {
        var id = ObjectId.GenerateNewId();
        const string name = "Tư vấn cá nhân";
        var avatar = Image.Of("avatar", appDefaultSettings.Value.GroupAvatarDefault);
        var status = isOpened ? ConversationStatus.Open : ConversationStatus.Closed;
        return Conversation.CreateConsultation(id, name, avatar, users.Select(u => u.UserId).ToList(), status);
    }

    private static List<Participant> MapToParticipants(ObjectId conversationId, HashSet<User> users)
    {
        var participants = users.Select(u => u.Role is Role.Doctor
            ? Participant.CreateDoctor(ObjectId.GenerateNewId(), u.UserId, conversationId, u.FullName, u.Avatar,
                u.PhoneNumber, u.Email)
            : Participant.CreateMember(ObjectId.GenerateNewId(), u.UserId, conversationId, u.FullName, u.Avatar,
                u.PhoneNumber, u.Email)).ToList();
        return participants;
    }
    
    private static ConversationLinkedEvent MapToConversationLinkedEvent(Conversation conversation, string consultationId)
    {
        return new ConversationLinkedEvent(conversation.Id.ToString(), consultationId, conversation.Status is ConversationStatus.Open);
    }
    
    private static ConversationCreatedEvent MapToConversationCreatedEvent(Conversation conversation, IEnumerable<User> users, string consultationId)
    {
        var memberIds = users.Select(u => u.UserId.ToString());
        return new ConversationCreatedEvent(
            conversation.Id.ToString(),
            conversation.Name, 
            conversation.ConversationType,
            conversation.Avatar.ToString(), 
            memberIds,
            conversation.Status is ConversationStatus.Open,
            consultationId);
    }
}