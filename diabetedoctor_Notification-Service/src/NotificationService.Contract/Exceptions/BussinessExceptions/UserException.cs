

namespace NotificationService.Contract.Exceptions.BussinessExceptions;

public class UserException
{
    public sealed class UserNotFoundException() : NotFoundException(
        UserMessage.UserNotFoundException.GetMessage().Message,
        UserMessage.UserNotFoundException.GetMessage().Code);
}