using ConsultationService.Contract.DTOs.ValueObjectDtos;
using ConsultationService.Domain.ValueObjects;

namespace ConsultationService.Application.Mapping;

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