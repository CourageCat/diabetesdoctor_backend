namespace UserService.Contract.DTOs.ServicePackage;

public class PackageFeatureDto
{
    public Guid? Id { get; init; }
    public PackageFeatureTypeEnum Type { get; init; }
    public string? Name { get; init; } = null!;
    public object? FeatureValue { get; init; } = null!;
    public DateTime? CreatedDate  { get; init; }
}