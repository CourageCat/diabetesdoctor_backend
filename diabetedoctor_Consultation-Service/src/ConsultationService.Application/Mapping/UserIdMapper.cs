using ConsultationService.Contract.DTOs.ValueObjectDtos;
using ConsultationService.Domain.ValueObjects;

namespace ConsultationService.Application.Mapping;

public static class UserIdMapper
{
    public static UserId ToDomain(this UserIdDto dto) => UserId.Of(dto.Id);
}