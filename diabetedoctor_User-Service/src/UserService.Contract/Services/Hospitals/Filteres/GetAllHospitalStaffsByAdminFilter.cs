namespace UserService.Contract.Services.Hospitals.Filteres;

public record GetAllHospitalStaffsByAdminFilter
{
    public string? Search { get; init; }
    public GenderEnum? Gender { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}