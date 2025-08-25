using NotificationService.Application.Mapping;
using NotificationService.Contract.DTOs.FcmDtos;
using NotificationService.Contract.Enums;
using NotificationService.Contract.Infrastructure;
using NotificationService.Contract.Services.Notification;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.UseCases.V1.Commands.Notifications;

public class CreatePostNotificationCommandHandler(
    INotificationRepository notificationRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IFirebaseNotificationService<PostFcmDto> firebaseNotificationService)
    : ICommandHandler<CreatePostNotificationCommand>
{
    public async Task<Result> Handle(CreatePostNotificationCommand request, CancellationToken cancellationToken)
    {
        var userIds = await userRepository.GetUserIdsAsync(cancellationToken: cancellationToken);
        if (userIds.Count == 0)
        {
            throw new UserException.UserNotFoundException();
        }
        
        var deviceIds = await userRepository.GetDeviceIdsAsync(userIds, cancellationToken: cancellationToken);
        if (deviceIds.Count == 0)
        {
            throw new UserException.UserNotFoundException();
        }

        var payload = new BsonDocument
        {
            { "postId", request.PostId },
            { "title", request.Title },
            { "thumbnail", request.Thumbnail }
        };

        var notifications = MapToNotifications(userIds, request, payload);
        
        await unitOfWork.StartTransactionAsync();

        try
        {
            await notificationRepository.CreateManyAsync(unitOfWork.ClientSession, notifications, cancellationToken: cancellationToken);

            var postPush = new PostFcmDto
            {
                Title = request.Title,
                Body = "Đã đăng bài viết mới",
                Icon = request.Thumbnail,
                PostId = request.PostId
            };

            //// push fcm
            await firebaseNotificationService.PushNotificationMultiDeviceAsync(deviceIds, postPush);

            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync();
            throw;
        }

        return Result.Success(new Success(
            NotificationMessage.SendNotificationSuccessfully.GetMessage().Code,
            NotificationMessage.SendNotificationSuccessfully.GetMessage().Message
        ));
    }

    private IEnumerable<Domain.Models.Notification> MapToNotifications(IEnumerable<UserId> userIds, CreatePostNotificationCommand command, BsonDocument payload)
    {
        var type = command.NotificationTypeEnum.ToEnum<NotificationTypeEnum, NotificationType>();
        return Domain.Models.Notification.CreateManyNotification(userIds, type, payload);
    }
}