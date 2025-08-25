
using NotificationService.Contract.Services.Notification;

namespace NotificationService.Application.UseCases.V1.Commands.Notifications;


public class MarkReadNotificationCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<MarkReadNotificationCommand>
{
    public async Task<Result> Handle(MarkReadNotificationCommand request, CancellationToken cancellationToken)
    {
        var objectIds = request.NotificationIds.Select(ObjectId.Parse);
        
        await unitOfWork.StartTransactionAsync();
        
        try
        {
            var updateDefinition = Builders<Domain.Models.Notification>.Update.Set(n => n.IsRead, true);

            await notificationRepository.UpdateManyAsync(
                unitOfWork.ClientSession,
                n => objectIds.Contains(n.Id) && n.UserId.Id.Equals(request.UserId),
                updateDefinition,
                cancellationToken
            );

            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {   
            await unitOfWork.AbortTransactionAsync();
            throw;
        }
        return Result.Success(new Success(
            NotificationMessage.UpdatedNotificationSuccessfully.GetMessage().Code,
            NotificationMessage.UpdatedNotificationSuccessfully.GetMessage().Message
        ));
    }
}