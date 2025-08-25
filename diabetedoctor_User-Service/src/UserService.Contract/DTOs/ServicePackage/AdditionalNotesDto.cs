namespace UserService.Contract.DTOs.ServicePackage;

public sealed record AdditionalNotesDto(string Type, string Value)
    : PackageFeatureValueDto(Type);