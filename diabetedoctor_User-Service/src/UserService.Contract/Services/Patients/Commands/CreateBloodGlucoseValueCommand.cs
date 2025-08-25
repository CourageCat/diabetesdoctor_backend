namespace UserService.Contract.Services.Patients.Commands;

public record CreateBloodGlucoseValueCommand
    (double Value, BloodGlucoseMeasureTimeEnum MeasureTime, string? PersonNote = null, DateTime? MeasurementAt = null, Guid? UserId = null, Guid? CarePlanInstanceId = null) : ICommand<Success<string>>;
