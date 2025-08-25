using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Attributes;
using ConsultationService.Contract.Common.Messages;

namespace ConsultationService.Contract.Common.DomainErrors;

public static class ConsultationErrors
{
    // error
    public static readonly Error NotFound = Error.NotFound(
        ConsultationMessage.ConsultationNotFound.GetMessage().Code,
        ConsultationMessage.ConsultationNotFound.GetMessage().Message);
    
    public static readonly Error ConsultationSessionsNotEnough = Error.BadRequest(
        ConsultationMessage.ConsultationSessionsNotEnough.GetMessage().Code,
        ConsultationMessage.ConsultationSessionsNotEnough.GetMessage().Message);
    
    
    
    // validation
    public static readonly Error BookingTimeExpired = Error.Validation(
        ConsultationMessage.BookingTimeExpired.GetMessage().Code,
        ConsultationMessage.BookingTimeExpired.GetMessage().Message);
    
    public static readonly Error CancelBookingTimeExpired = Error.Validation(
        ConsultationMessage.CancelBookingTimeExpired.GetMessage().Code,
        ConsultationMessage.CancelBookingTimeExpired.GetMessage().Message);
}