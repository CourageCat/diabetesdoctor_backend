namespace ConsultationService.Contract.Settings;

public class GrpcSettings
{
    public const string SectionName = "GrpcSettings";
    public string UserServiceAddress { get; init; } = null!;
}