namespace UserService.Contract.DTOs.ServicePackage;

public record PackageFeatureRequestDto
{
    public PackageFeatureTypeEnum Type { get; init; }
    public object Value { get; init; } = null!;
    public string Description { get; init; } = string.Empty;
}