namespace UserService.Contract.Services.Patients.Responses;

public record PatientProfileResponse
{
    public string Id { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public GenderEnum Gender { get; init; }
    public DiabetesEnum DiabetesType { get; init; }
    public DiagnosisInfoDto DiagnosisInfo { get; init; } = null!;
    public DiabetesConditionDto DiabetesCondition { get; init; } = null!;
    public IEnumerable<MedicalHistoryForDiabetesEnum> MedicalHistories { get; init; } = [];
}
