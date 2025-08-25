namespace UserService.Contract.Services.Patients.Commands;

/// <summary>
/// Command tạo mới hồ sơ bệnh nhân tiểu đường, bao gồm thông tin cá nhân, chẩn đoán, điều kiện điều trị và tiền sử.
/// </summary>
public record CreatePatientProfileCommand : ICommand<Success>
{
    // Thông tin cơ bản
    public string FirstName { get; init; } = null!;
    public string? MiddleName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public DateTime DateOfBirth { get; init; } // ISO format: "yyyy-MM-dd"
    public GenderEnum Gender { get; init; }
    public double HeightCm { get; init; }
    public double WeightKg { get; init; }
    public DiabetesEnum Diabetes { get; init; }

    // Thông tin chẩn đoán
    public DiagnosisRecencyEnum DiagnosisRecency { get; init; }
    public int? Year { get; init; }

    // Tình trạng điều trị hiện tại
    public TreatmentMethodEnum? Type2TreatmentMethod { get; init; }
    public ControlLevelEnum? ControlLevel { get; init; }
    public InsulinInjectionFrequencyEnum? InsulinInjectionFrequency { get; init; }
    public List<ComplicationEnum>? Complications { get; init; } = [];
    public string? OtherComplicationDescription { get; init; }
    public ExerciseFrequencyEnum? ExerciseFrequency { get; init; }
    public EatingHabitEnum? EatingHabit { get; init; }
    public bool? UsesAlcoholOrTobacco { get; init; }

    // Tiền sử bệnh nền liên quan
    public List<MedicalHistoryForDiabetesEnum>? MedicalHistories { get; init; } = [];

    public Guid UserId { get; init; }
}
