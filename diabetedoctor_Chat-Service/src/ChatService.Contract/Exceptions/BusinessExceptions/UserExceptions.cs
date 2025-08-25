using ChatService.Contract.Attributes;
using ChatService.Contract.Common.Messages;

namespace ChatService.Contract.Exceptions.BusinessExceptions;

public static class UserExceptions
{
    public sealed class UserNotFoundException() : NotFoundException(UserMessage.UserNotFound.GetMessage().Message,
        UserMessage.UserNotFound.GetMessage().Code);
}