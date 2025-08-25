

namespace NotificationService.Contract.Exceptions.BussinessExceptions;

public class NotificationException
{
    public sealed class NotificationNotFound() : NotFoundException(
        NotificationMessage.NotificationNotFoundException.GetMessage().Message,
        NotificationMessage.NotificationNotFoundException.GetMessage().Code);
}