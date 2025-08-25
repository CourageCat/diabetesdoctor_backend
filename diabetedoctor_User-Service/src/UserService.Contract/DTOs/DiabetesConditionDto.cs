namespace UserService.Contract.DTOs;

public record DiabetesConditionDto
{
    public DiabetesEnum DiabetesType { get; init; } 
    public TreatmentMethodEnum? Type2TreatmentMethod { get; init; } 
    
    public ControlLevelEnum? ControlLevel { get; init; } 
    public InsulinInjectionFrequencyEnum? InsulinFrequency { get; init; } 

    public bool HasComplications { get; init; } 
    public List<ComplicationEnum> Complications { get; init; } = []; 
    public string? OtherComplicationDescription { get; init; } 

    public ExerciseFrequencyEnum? ExerciseFrequency { get; init; } 
    public EatingHabitEnum? EatingHabit { get; init; } 
    public bool? UsesAlcoholOrTobacco { get; init; } 
}