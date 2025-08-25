using ChatService.Application.Mapping;
using ChatService.Contract.Attributes;
using ChatService.Contract.Enums;
using ChatService.Contract.EventBus.Abstractions;
using ChatService.Contract.EventBus.Events.MessageIntegrationEvents;
using ChatService.Contract.Services.Message.Commands;
using ChatService.Contract.Services.Message.Responses;

namespace ChatService.Application.UseCase.V1.Commands.Messages;

public class CreateMessageCommandHandler(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    IParticipantRepository participantRepository,
    IMediaRepository mediaRepository,
    IOutboxEventRepository outboxEventRepository,
    IOptions<KafkaSettings> kafkaSettings,
    IUnitOfWork unitOfWork,
    IAblyEventPublisher ablyEventPublisher)
    : ICommandHandler<CreateMessageCommand, Response<CreateMessageResponse>>
{
    public async Task<Result<Response<CreateMessageResponse>>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var senderId = UserId.Of(request.UserId);
        var participant = await GetParticipantAsync(senderId, request.ConversationId, cancellationToken);
        if (participant.IsFailure)
        {
            return Result.Failure<Response<CreateMessageResponse>>(participant.Error);
        }
        
        var conversation = await GetConversationAsync(request, cancellationToken);
        if (conversation.IsFailure)
        {
            return Result.Failure<Response<CreateMessageResponse>>(conversation.Error);
        }
        
        var id = ObjectId.GenerateNewId();
        Media? media = null;
        Message message;
        switch (request.MessageType)
        {
            case MessageTypeEnum.Text:
                message = Message.CreateText(id, request.ConversationId, senderId, request.Content!);
                break;
            case MessageTypeEnum.File:
                var mediaId = ObjectId.Parse(request.MediaId);
                media = await mediaRepository.FindByIdAsync(mediaId, cancellationToken);
                if (media is null)
                {
                    return Result.Failure<Response<CreateMessageResponse>>(MediaErrors.MediaNotFound);
                }

                var file = FileAttachment.Of(media.PublicId, media.PublicUrl, media.MediaType);
                message = Message.CreateFile(id, request.ConversationId, senderId, media.OriginalFileName,
                    file);
                media.Use();
                break;
            default:
                throw new NotSupportedException($"Unsupported message type: {request.MessageType.ToString()}");
        }

        var integrationEvent = MapToIntegrationEvent(senderId, conversation.Value, message);

        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            await messageRepository.CreateAsync(unitOfWork.ClientSession, message, cancellationToken);
            if (media is not null)
            {
                await mediaRepository.ReplaceOneAsync(unitOfWork.ClientSession, media, cancellationToken);
            }
            var @event = OutboxEventExtension.ToOutboxEvent(kafkaSettings.Value.ChatTopic, integrationEvent);
            await outboxEventRepository.CreateAsync(unitOfWork.ClientSession, @event, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }

        // await ablyEventPublisher.PublishAsync(AblyTopicConstraints.GlobalChatChannel,
        //     AblyTopicConstraints.GlobalChatEvent, integrationEvent);
        return Result.Success(new Response<CreateMessageResponse>(
            MessageMessage.CreateMessageSuccessfully.GetMessage().Code,
            MessageMessage.CreateMessageSuccessfully.GetMessage().Message,
            new CreateMessageResponse(message.Id.ToString())));
    }

    private async Task<Result<Conversation>> GetConversationAsync(
        CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var conversationType = request.ConversationType.ToEnum<ConversationTypeEnum, ConversationType>();
        var projection = Builders<Conversation>.Projection
            .Include(c => c.Status);
            // .Include(c => c.Name)
            // .Include(c => c.Avatar)
            // .Include(c => c.ConversationType)
        var conversation = await conversationRepository.FindSingleAsync(
            c => c.Id == request.ConversationId
                 && c.ConversationType == conversationType
                 && c.IsDeleted == false,
            projection, cancellationToken);

        if (conversation is null)
            return Result.Failure<Conversation>(ConversationErrors.NotFound);

        return conversation.Status is ConversationStatus.Open 
            ? Result.Success(conversation)
            : Result.Failure<Conversation>(ConversationErrors.ThisConversationIsClosed);
    }

    private async Task<Result> GetParticipantAsync(UserId senderId, ObjectId conversationId, CancellationToken cancellationToken)
    {
        var participantExisted = await participantRepository.ExistsAsync(
            p => p.UserId == senderId
                 && p.ConversationId == conversationId
                 && p.ParticipantStatus == ParticipantStatus.Active
                 && p.IsDeleted == false,
            cancellationToken);

        return participantExisted 
            ? Result.Success()
            : Result.Failure<Participant>(ParticipantErrors.ParticipantNotExistOrBanned) ;
    }
    
    private MessageCreatedIntegrationEvent MapToIntegrationEvent(UserId senderId, Conversation conversation, Message message)
    {
        return new MessageCreatedIntegrationEvent
        {
            // Sender = new SenderInfo
            // {
            //     SenderId = participant.UserId.Id,
            //     FullName = participant.DisplayName,
            //     Avatar = participant.Avatar.PublicUrl
            // },
            // Conversation = new ConversationInfo
            // {
            //     ConversationId = conversation.Id.ToString(),
            //     ConversationType = (int)conversation.ConversationType
            // },
            SenderId = senderId.ToString(),
            ConversationId = conversation.Id.ToString(),
            ConversationType = conversation.ConversationType.ToEnum<ConversationType, ConversationTypeEnum>(),
            MessageId = message.Id.ToString(),
            MessageContent = message.Content,
            MessageType = message.Type.ToEnum<MessageType, MessageTypeEnum>(),
            FileAttachment = Mapper.MapFileAttachmentDto(message.File),
            CreatedDate = message.CreatedDate
        };
        
        // return new MessageCreatedIntegrationEvent
        // {
        //     Sender = new SenderInfo
        //     {
        //         SenderId = participant.UserId.Id,
        //         FullName = participant.DisplayName,
        //         Avatar = participant.Avatar.PublicUrl
        //     },
        //     Conversation = conversation.ConversationType switch
        //     {
        //         ConversationType.Group => new ConversationInfo
        //         {
        //             ConversationId = conversation.Id.ToString(),
        //             // ConversationName = conversation.Name,
        //             // Avatar = conversation.Avatar.PublicUrl,
        //             ConversationType = (int)ConversationType.Group
        //         },
        //         _ => new ConversationInfo
        //         {
        //             ConversationId = conversation.Id.ToString(),
        //             ConversationType = (int)conversation.ConversationType
        //         }
        //     },
        //     MessageId = message.Id.ToString(),
        //     MessageContent = message.Content,
        //     MessageType = (int)message.Type,
        //     FileAttachment = Mapper.MapFileAttachmentDto(message.File),
        //     CreatedDate = message.CreatedDate
        // };
    }
}