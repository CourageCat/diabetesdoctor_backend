using UserService.Contract.Enums.Doctor;

namespace UserService.Contract.Services.Hospitals.Commands;

/// <summary>
/// Command tạo mới hồ sơ bệnh nhân tiểu đường, bao gồm thông tin cá nhân, chẩn đoán, điều kiện điều trị và tiền sử.
/// </summary>
public record CreateDoctorCommand : ICommand<Success>
{
    public string PhoneNumber { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string? MiddleName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public DateTime DateOfBirth { get; init; } // ISO format: "yyyy-MM-dd"
    public GenderEnum Gender { get; init; }
    public Guid AvatarId { get; init; }

    public int NumberOfExperiences { get; init; }
    public DoctorPositionEnum Position { get; init; }
    public string Introduction { get; init; } = null!;
    public Guid HospitalStaffId { get; init; }
}