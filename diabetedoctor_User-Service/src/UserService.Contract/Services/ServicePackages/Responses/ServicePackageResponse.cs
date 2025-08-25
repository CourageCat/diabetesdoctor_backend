namespace UserService.Contract.Services.ServicePackages.Responses;

public record ServicePackageResponse
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public double Price { get; init; }
    public int Sessions { get; init; }
    public int DurationInMonths { get; init; } 
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
}