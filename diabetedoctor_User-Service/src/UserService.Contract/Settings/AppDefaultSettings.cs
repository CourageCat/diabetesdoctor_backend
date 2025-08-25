namespace UserService.Contract.Settings;

public class AppDefaultSettings
{
    public const string SectionName = "AppDefaultSettings";
    public string AvatarPatientId { get; init; } = null!;
    public string AvatarPatientUrl { get; init; } = null!;
    public string AvatarDoctorId { get; init; } = null!;
    public string AvatarDoctorUrl { get; init; } = null!;
    public string AvatarStaffId { get; init; } = null!;
    public string AvatarStaffUrl { get; init; } = null!;
    public string AvatarAdminId { get; init; } = null!;
    public string AvatarAdminUrl { get; init; } = null!;
}

