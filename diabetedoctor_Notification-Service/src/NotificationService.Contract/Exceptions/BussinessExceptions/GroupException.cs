namespace NotificationService.Contract.Exceptions.BussinessExceptions;

public static class GroupException
{
    public sealed class GroupNotFoundException() : NotFoundException(
        GroupMessage.GroupNotFoundException.GetMessage().Message,
        GroupMessage.GroupNotFoundException.GetMessage().Code);
}