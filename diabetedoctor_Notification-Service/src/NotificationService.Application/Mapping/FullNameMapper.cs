using NotificationService.Contract.DTOs.ValueObjectDtos;

namespace NotificationService.Application.Mapping;

public static class FullNameMapper
{
    public static FullNameDto ToDto(this FullName fullName)
    {
        return new FullNameDto
        {
            LastName = fullName.LastName,
            MiddleName = fullName.MiddleName,
            FirstName = fullName.FirstName
        };
    }
    
    public static FullName ToDomain(this FullNameDto dto)
    {
        return FullName.Create(dto.LastName, dto.MiddleName, dto.FirstName);
    }
}