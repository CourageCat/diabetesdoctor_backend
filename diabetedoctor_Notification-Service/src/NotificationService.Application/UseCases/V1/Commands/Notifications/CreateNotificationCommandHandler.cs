using NotificationService.Contract.DTOs.FcmDtos;
using NotificationService.Contract.Enums;
using NotificationService.Contract.Infrastructure;
using NotificationService.Contract.Services.Notification;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.UseCases.V1.Commands.Notifications;

public class CreateGroupCommandHandler(
    INotificationRepository notificationRepo,
    IUnitOfWork unitOfWork,
    IFirebaseNotificationService<PostFcmDto> firebaseNotificationService,
    IUserRepository userRepo) 
    : ICommandHandler<CreateNotificationCommand>
{
    public async Task<Result> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var userIds = request.UserIds.Any()
            ? request.UserIds.Select(UserId.Of).ToList()
            : await userRepo.GetUserIdsAsync(cancellationToken);
            
        if (userIds.Count == 0)
        {
            throw new UserException.UserNotFoundException();
        }

        var deviceIds = await userRepo.GetDeviceIdsAsync(userIds, cancellationToken: cancellationToken);
        if (deviceIds.Count == 0)
        {
            throw new UserException.UserNotFoundException();
        }

        var payload = new BsonDocument
             {
                 { "title", request.Title },
                 { "thumbnail", request.Thumbnail},
                 { "body", request.Body },
                 { "createdBy", request.SenderId}
             };

        var notifications = MapToNotifications(userIds, request, payload);

        await unitOfWork.StartTransactionAsync();

        try
        {
            await notificationRepo.CreateManyAsync(unitOfWork.ClientSession, notifications, cancellationToken);

            var postPush = new PostFcmDto
            {
                Title = request.Title,
                Body = "Đã đăng bài viết mới",
                Icon = request.Thumbnail
            };

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
            NotificationMessage.SendNotificationSuccessfully.GetMessage().Message));
    }
    private IEnumerable<Domain.Models.Notification> MapToNotifications(IEnumerable<UserId> userIds, CreateNotificationCommand command, BsonDocument payload)
    {
        var type = command.NotificationTypeEnum.ToEnum<NotificationTypeEnum, NotificationType>();
        return Domain.Models.Notification.CreateManyNotification(userIds, type, payload);
    }
}