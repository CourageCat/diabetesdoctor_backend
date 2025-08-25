using UserService.Contract.Enums.Doctor;

namespace UserService.Contract.Services.Doctors.Commands;

public record UpdateDoctorProfileCommand : ICommand<Success>
{
    public Guid UserId { get; init; }
    public string? FirstName { get; init; } = null!;
    public string? MiddleName { get; init; } = null!;
    public string? LastName { get; init; } = null!;
    public DateTime? DateOfBirth { get; init; } // ISO format: "yyyy-MM-dd"
    public GenderEnum? Gender { get; init; }
    public int? NumberOfExperiences  { get; init; }
    public DoctorPositionEnum? Position { get; init; }
    public string? Introduction  { get; init; } = null!;
}