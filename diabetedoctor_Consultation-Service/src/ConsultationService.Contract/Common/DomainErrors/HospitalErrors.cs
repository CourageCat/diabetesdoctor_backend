using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Attributes;
using ConsultationService.Contract.Common.Messages;

namespace ConsultationService.Contract.Common.DomainErrors;

public static class HospitalErrors
{
    public static readonly Error HospitalNotFound = Error.BadRequest(
        HospitalMessage.HospitalNotFound.GetMessage().Code, 
        HospitalMessage.HospitalNotFound.GetMessage().Message);
}