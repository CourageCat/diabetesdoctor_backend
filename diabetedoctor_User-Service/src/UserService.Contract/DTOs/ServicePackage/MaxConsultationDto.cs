namespace UserService.Contract.DTOs.ServicePackage;

public sealed record MaxConsultationDto(string Type, int Value)
    : PackageFeatureValueDto(Type);