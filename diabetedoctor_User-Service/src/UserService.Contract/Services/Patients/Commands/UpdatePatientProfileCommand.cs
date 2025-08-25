namespace UserService.Contract.Services.Patients.Commands;

public record UpdatePatientProfileCommand : ICommand<Success>
{
    public Guid UserId { get; init; }
    public string? FirstName { get; init; } = null!;
    public string? MiddleName { get; init; } = null!;
    public string? LastName { get; init; } = null!;
    public DateTime? DateOfBirth { get; init; } // ISO format: "yyyy-MM-dd"
    public GenderEnum? Gender { get; init; }
}