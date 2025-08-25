namespace UserService.Contract.Services.Patients.Commands;

public record CreateBloodPressureValueCommand
    (double Systolic, double Diastolic, string? PersonNote = null, DateTime? MeasurementAt = null, Guid? UserId = null, Guid? CarePlanInstanceId = null) : ICommand<Success<string>>;
