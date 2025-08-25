using MediaService.Contract.Common.Messages;
using MediaService.Contract.Exceptions;
using MediaService.Contract.Helpers;

namespace MediaService.Contract.Exceptions.BussinessExceptions;

public static class UserException
{
    public sealed class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException()
                : base(UserMessage.UserNotFoundException.GetMessage().Message,
                    UserMessage.UserNotFoundException.GetMessage().Code)
        { }
    }
}
