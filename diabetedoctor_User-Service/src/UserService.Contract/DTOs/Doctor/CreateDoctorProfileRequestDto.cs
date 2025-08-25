using UserService.Contract.Enums.Doctor;

namespace UserService.Contract.DTOs.Doctor;

public sealed record CreateDoctorProfileRequestDto
{
    public string FirstName { get; init; } = null!;
    public string? MiddleName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public DateTime DateOfBirth { get; init; } // ISO format: "yyyy-MM-dd"
    public GenderEnum Gender { get; init; }
    public Guid Avatar { get; init; }

    public int NumberOfExperiences { get; init; }
    public DoctorPositionEnum PositionEnum { get; init; }
    public string Introduction { get; init; } = null!;
    public IEnumerable<Guid> Images { get; init; } = null!;
}