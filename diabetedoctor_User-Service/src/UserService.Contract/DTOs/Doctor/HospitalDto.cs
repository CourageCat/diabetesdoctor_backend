namespace UserService.Contract.DTOs.Doctor;

public record HospitalDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string Thumbnail { get; init; } = null!;
}