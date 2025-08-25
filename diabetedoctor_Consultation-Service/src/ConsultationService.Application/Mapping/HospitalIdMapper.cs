using ConsultationService.Contract.DTOs.ValueObjectDtos;
using ConsultationService.Domain.ValueObjects;

namespace ConsultationService.Application.Mapping;

public static class HospitalIdMapper
{
    public static HospitalId ToDomain(this HospitalIdDto dto) => HospitalId.Of(dto.Id);
}