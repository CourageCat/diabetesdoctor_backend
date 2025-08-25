using NotificationService.Contract.Services.Notification;

namespace NotificationService.Application.UseCases.V1.Commands.Notifications;


public class DeleteNotificationCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteNotificationCommand>
{
    public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var id = ObjectId.Parse(request.NotificationId);
        var notification = await notificationRepository.ExistAsync(
            notification => notification.Id == id && notification.UserId.Id == request.UserId, cancellationToken);

        if (!notification)
        {
            throw new NotificationException.NotificationNotFound();
        }

        await unitOfWork.StartTransactionAsync();
        try
        {
            await notificationRepository.DeleteAsync(unitOfWork.ClientSession, id, cancellationToken: cancellationToken);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync();
            throw;
        }
            
        return Result.Success(new Success(GroupMessage.DeletedGroupSuccessfully.GetMessage().Code, GroupMessage.DeletedGroupSuccessfully.GetMessage().Message));
    }
}