using NotificationService.Contract.DTOs.FcmDtos;
using NotificationService.Contract.Enums;
using NotificationService.Contract.Infrastructure;
using NotificationService.Contract.Services.Notification.Commands;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases.V1.Commands.Notifications;

public class PushChatNotificationCommandHandler(
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    IFirebaseNotificationService<BaseFcmDto> firebaseNotificationService)
    : ICommandHandler<PushChatNotificationCommand>
{
    public async Task<Result> Handle(PushChatNotificationCommand request, CancellationToken cancellationToken)
    {
        var conversationId = ConversationId.Of(request.ConversationId);
        // var conversationProjection = Builders<Conversation>.Projection.
        
        var conversation = await conversationRepository.FindSingleAsync(c => c.ConversationId == conversationId,
            cancellationToken: cancellationToken);
        if (conversation is null)
        {
            throw new GroupException.GroupNotFoundException();
        }

        var senderId = UserId.Of(request.SenderId);
        var user = await userRepository.FindSingleAsync(
            u => u.UserId == senderId && u.IsDeleted == false,
            cancellationToken: cancellationToken);
        if (user is null)
        {
            throw new UserException.UserNotFoundException();
        }
        
        //except sender id
        // var recipientMembers = conversation.Members.Where(m => m.Id != request.SenderId).ToList();
        
        // var deviceIds = await userRepository.GetDeviceIdsAsync(recipientMembers, cancellationToken);

        var deviceIds = await userRepository.GetDeviceIdsAsync(conversation.Members, cancellationToken);
        
        if (deviceIds.Count == 0)
        {
            throw new UserException.UserNotFoundException();
        }

        var fcmDto = request.ConversationType switch
        {
            ConversationTypeEnum.Group => new ChatFcmDto()
            {
                Title = conversation.Name,
                Body = user.FullName + ": " + request.MessageContent,
                Icon = "https://plus.unsplash.com/premium_photo-1681843126728-04eab730febe?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                ConversationId = request.ConversationId,
                ConversationName = conversation.Name,
                ConversationAvatar = conversation.Avatar.ToString(),
                ConversationType = (int)ConversationTypeEnum.Group,
                SenderId = user.UserId.ToString(),
                SenderName = user.FullName,
                SenderAvatar = user.Avatar.ToString(),
                MessageId = request.MessageId,
                MessageContent = request.MessageContent,
                MessageType = (int)request.MessageType,
                FileUrl = request.FileAttachment?.PublicUrl,
                FileType = request.FileAttachment is not null ? (int)request.FileAttachment.Type : null,
                CreatedDate = request.CreatedDate
            } as BaseFcmDto,
            ConversationTypeEnum.Personal => new ChatFcmDto
            {
                Title = user.FullName,
                Body = request.MessageContent,
                Icon = "https://plus.unsplash.com/premium_photo-1681843126728-04eab730febe?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                ConversationId = request.ConversationId,
                ConversationAvatar = user.Avatar.ToString(),
                ConversationType = (int)ConversationTypeEnum.Personal,
                SenderId = user.UserId.ToString(),
                SenderName = user.FullName,
                SenderAvatar = user.Avatar.ToString(),
                MessageId = request.MessageId,
                MessageContent = request.MessageContent,
                MessageType = (int)request.MessageType,
                FileUrl = request.FileAttachment?.PublicUrl,
                FileType = request.FileAttachment is not null ? (int)request.FileAttachment.Type : null,
                CreatedDate = request.CreatedDate
            } as BaseFcmDto,
            _ => throw new ArgumentOutOfRangeException()
        };

        await firebaseNotificationService.PushNotificationMultiDeviceAsync(deviceIds!, fcmDto);
        
        return Result.Success();
    }
}