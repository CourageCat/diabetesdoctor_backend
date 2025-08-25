namespace UserService.Contract.DTOs.ServicePackage;

public record ServicePackageDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public double Price { get; init; }
    public DateTime CreatedDate { get; init; }
    public PackageTypeDto Type { get; init; } = null!;
}