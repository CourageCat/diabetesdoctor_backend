namespace UserService.Domain.ValueObjects;

/// <summary>
/// Giá trị mô tả tình trạng bệnh tiểu đường hiện tại của bệnh nhân.
/// Bao gồm loại tiểu đường, phương pháp điều trị, biến chứng, và lối sống.
/// </summary>
public sealed class DiabetesCondition : ValueObject
{
    public DiabetesType DiabetesType { get; } // Loại tiểu đường
    public TreatmentMethodType? Type2TreatmentMethod { get; } // Phương pháp điều trị cho loại 2
    
    public ControlLevelType? ControlLevel { get; } // Khả năng kiểm soát chỉ số hbA1c
    public InsulinInjectionFrequencyType? InsulinFrequency { get; } // Tần suất tiêm Insulin

    public bool HasComplications { get; } // Có biến chứng không?
    public List<ComplicationType> Complications { get; } = []; // List biến chứng
    public string? OtherComplicationDescription { get; } // Biến chứng khác (nếu có)

    public ExerciseFrequencyType? ExerciseFrequency { get; } // Tần suất tập thể dục
    public EatingHabitType? EatingHabit { get; } // Thói quen ăn uống
    public bool? UsesAlcoholOrTobacco { get; } // Có sử dụng thuốc lá, rượu bia không?

    private DiabetesCondition() { }

    private DiabetesCondition(
        DiabetesType diabetesType,
        InsulinInjectionFrequencyType? insulinFrequency,
        TreatmentMethodType? type2TreatmentMethod,
        ControlLevelType? controlLevel,
        bool hasComplications,
        List<ComplicationType>? complications,
        string? otherComplicationDescription,
        ExerciseFrequencyType? exerciseFrequency,
        EatingHabitType? eatingHabit,
        bool? usesAlcoholOrTobacco)
    {
        DiabetesType = diabetesType;
        InsulinFrequency = insulinFrequency;
        Type2TreatmentMethod = type2TreatmentMethod;
        ControlLevel = controlLevel;
        HasComplications = hasComplications;
        Complications = complications ?? [];
        OtherComplicationDescription = otherComplicationDescription;
        ExerciseFrequency = exerciseFrequency;
        EatingHabit = eatingHabit;
        UsesAlcoholOrTobacco = usesAlcoholOrTobacco;
    }
    public static DiabetesCondition Of(
        DiabetesType diabetesType,
        InsulinInjectionFrequencyType? insulinFrequency,
        TreatmentMethodType? type2TreatmentMethod,
        ControlLevelType? controlLevel,
        bool hasComplications,
        List<ComplicationType>? complications,
        string? otherComplicationDescription,
        ExerciseFrequencyType? exerciseFrequency,
        EatingHabitType? eatingHabit,
        bool? usesAlcoholOrTobacco)
    {
        if (diabetesType == DiabetesType.Type1 && insulinFrequency == null)
            throw new ArgumentException("Bệnh tiểu đường loại 1 phải có tần suất insulin.");

        if (diabetesType == DiabetesType.Type2 && type2TreatmentMethod == null)
            throw new ArgumentException("Bệnh tiểu đường loại 2 phải chỉ định phương pháp điều trị.");

        return new DiabetesCondition(
            diabetesType,
            insulinFrequency,
            type2TreatmentMethod,
            controlLevel,
            hasComplications,
            complications,
            otherComplicationDescription,
            exerciseFrequency,
            eatingHabit,
            usesAlcoholOrTobacco);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DiabetesType;
        yield return InsulinFrequency;
        yield return Type2TreatmentMethod;
        yield return ControlLevel;
        yield return HasComplications;
        yield return OtherComplicationDescription ?? string.Empty;
        yield return ExerciseFrequency;
        yield return EatingHabit;
        yield return UsesAlcoholOrTobacco ?? false;

        foreach (var c in Complications.OrderBy(x => x))
            yield return c;
    }
}
