namespace UserService.Contract.Services.ServicePackages.Commands;

public record CreateServicePackageCommand : ICommand<Success>
{
    public Guid AdminId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public double Price { get; init; }
    public int Sessions { get; init; }
    public int DurationInMonths  { get; init; }
}