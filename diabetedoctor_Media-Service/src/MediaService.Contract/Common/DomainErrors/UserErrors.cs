using MediaService.Contract.Common.Messages;

namespace MediaService.Contract.Common.DomainErrors;

public static class UserErrors
{
    public static readonly Error UserNotFoundException = Error.Conflict(UserMessage.UserNotFoundException.GetMessage().Code,
        UserMessage.UserNotFoundException.GetMessage().Message);
}