using NotificationService.Contract.DTOs.ValueObjectDtos;

namespace NotificationService.Application.Mapping;

public static class UserIdMapper
{
    public static UserId ToDomain(this UserIdDto dto) => UserId.Of(dto.Id);
}