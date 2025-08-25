namespace UserService.Contract.Services.Hospitals.Commands;

public record CreateHospitalStaffCommand : ICommand<Success>
{
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string? MiddleName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public DateTime DateOfBirth { get; init; } // ISO format: "yyyy-MM-dd"
    public GenderEnum Gender { get; init; }
    public Guid AvatarId { get; init; }
    public Guid HospitalAdminId { get; init; }
}