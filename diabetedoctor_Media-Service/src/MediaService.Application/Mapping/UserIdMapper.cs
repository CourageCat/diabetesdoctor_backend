using MediaService.Contract.DTOs.ValueObjectDtos;
using MediaService.Domain.ValueObjects;

namespace MediaService.Application.Mapping;

public static class UserIdMapper
{
    public static UserId ToDomain(this UserIdDto dto) => UserId.Of(dto.Id);
}