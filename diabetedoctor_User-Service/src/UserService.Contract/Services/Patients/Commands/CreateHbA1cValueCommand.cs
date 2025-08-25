namespace UserService.Contract.Services.Patients.Commands;

public record CreateHbA1cValueCommand
    (double Value, string? PersonNote = null, DateTime? MeasurementAt = null, Guid? UserId = null, Guid? CarePlanInstanceId = null) : ICommand<Success<string>>;
