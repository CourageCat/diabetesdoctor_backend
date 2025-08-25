using UserService.Contract.DTOs.Doctor;

namespace UserService.Contract.Services.Hospitals.Responses;

public record HospitalStaffResponse
{
    public string Id { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string Name { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public GenderEnum Gender { get; init; }
    public HospitalDto? Hospital { get; init; }
    public DateTime? CreatedDate { get; init; }
}