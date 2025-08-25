using ConsultationService.Contract.Attributes;
using ConsultationService.Contract.Common.Messages;

namespace ConsultationService.Contract.Exceptions.BusinessExceptions;

public static class ConsultationExceptions
{
    public sealed class ConsultationNotFoundException() : NotFoundException(ConsultationMessage.ConsultationNotFound.GetMessage().Message,
        ConsultationMessage.ConsultationNotFound.GetMessage().Code);
}