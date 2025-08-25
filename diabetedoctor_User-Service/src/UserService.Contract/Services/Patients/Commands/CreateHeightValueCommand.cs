namespace UserService.Contract.Services.Patients.Commands;

public record CreateHeightValueCommand
    (double Value, DateTime? MeasurementAt = null, Guid? UserId = null, Guid? CarePlanInstanceId = null) : ICommand<Success<string>>;
