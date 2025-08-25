namespace UserService.Contract.Services.Patients.Responses;

public record CheckSessionRemainingResponse
{
    public double Price { get; init; }
    public Guid UserPackageId { get; init; }
}