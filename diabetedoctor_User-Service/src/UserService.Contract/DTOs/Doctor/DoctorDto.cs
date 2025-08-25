using UserService.Contract.Enums.Doctor;

namespace UserService.Contract.DTOs.Doctor;

public record DoctorDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Avatar  { get; init; } = null!;
}