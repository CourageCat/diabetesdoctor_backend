using UserService.Contract.Enums.Doctor;

namespace UserService.Contract.Services.Doctors.Filters;

public class GetAllDoctorsFilter
{
    public string? Search { get; init; }
    public GenderEnum? Gender { get; init; }
    public DoctorPositionEnum? Position { get; init; }
    public Guid? HospitalId { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}