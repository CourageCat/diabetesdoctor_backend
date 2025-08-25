namespace UserService.Contract.Services.Hospitals.Commands;

public record UpdateHospitalCommand : ICommand<Success>
{
    public Guid Id { get; init; }
    public string? Name { get; init; } = null!;
    public string? Email { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!; 
    public string? Website { get; init; } = null!;
    public string? Address { get; init; } = null!;
    public string? Introduction { get; init; } = null!;
    public Guid? Thumbnail { get; init; }
    public List<Guid> Images { get; init; } = null!;
}