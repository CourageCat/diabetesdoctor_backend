namespace UserService.Contract.DTOs.ServicePackage;

public record PackageTypeDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
}