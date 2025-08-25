using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Attributes;
using ConsultationService.Contract.Common.Messages;

namespace ConsultationService.Contract.Common.DomainErrors;

public static class ConsultationTemplateErrors
{
    // validation
    public static readonly Error MinimumDuration = Error.Validation(
        ConsultationTemplateMessage.MinimumDuration.GetMessage().Code,
        ConsultationTemplateMessage.MinimumDuration.GetMessage().Message);
    
    public static readonly Error StartTimeAfterEndTime = Error.Validation(
        ConsultationTemplateMessage.StartTimeAfterEndTime.GetMessage().Code,
        ConsultationTemplateMessage.StartTimeAfterEndTime.GetMessage().Message);
    
    // error
    public static readonly Error NotFound = Error.NotFound(
        ConsultationTemplateMessage.TemplateNotFound.GetMessage().Code,
        ConsultationTemplateMessage.TemplateNotFound.GetMessage().Message);
    
    public static readonly Error TemplateIsBooked = Error.Validation(
        ConsultationTemplateMessage.TemplateIsBooked.GetMessage().Code,
        ConsultationTemplateMessage.TemplateIsBooked.GetMessage().Message);
}