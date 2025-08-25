namespace UserService.Contract.Services.Patients.Commands;

public record CreateWeightValueCommand
    (double Value, DateTime? MeasurementAt = null, Guid? UserId = null, Guid? CarePlanInstanceId = null) : ICommand<Success<string>>;
