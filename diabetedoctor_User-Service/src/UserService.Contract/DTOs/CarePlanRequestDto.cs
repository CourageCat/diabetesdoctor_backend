namespace UserService.Contract.DTOs;

public record CarePlanRequestDto(
    string PatientId,
    int Age,
    string Gender,
    float Bmi,
    string DiabetesType,
    string InsulinSchedule,
    string TreatmentMethod,
    List<string> Complications,
    List<string> PastDiseases,
    string Lifestyle
);
