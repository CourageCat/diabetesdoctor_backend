using ConsultationService.Contract.Attributes;
using ConsultationService.Contract.Common.Messages;

namespace ConsultationService.Contract.Exceptions.BusinessExceptions;

public static class UserExceptions
{
    public sealed class UserNotFoundException() : NotFoundException(UserMessage.UserNotFound.GetMessage().Message,
        UserMessage.UserNotFound.GetMessage().Code);
}