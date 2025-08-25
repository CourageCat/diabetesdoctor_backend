namespace AuthService.Api.Features.Hospital.Commands;

public record HospitalAdminCreatedProfileCommand : ICommand<Success>
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string Email { get; init; } = null!;
}