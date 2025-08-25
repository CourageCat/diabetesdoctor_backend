using UserService.Contract.DTOs.Doctor;
using UserService.Contract.Enums.Doctor;

namespace UserService.Contract.Services.Doctors.Responses;

public record DoctorResponse
{
    public string Id { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string Name { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public GenderEnum Gender { get; init; }
    public int NumberOfExperiences { get; init; }
    public DoctorPositionEnum Position { get;  init; }
    public string Introduction { get; init; } = null!;
    public HospitalDto? Hospital { get; init; } = null!;
    public DateTime? CreatedDate { get; init; }
    
}