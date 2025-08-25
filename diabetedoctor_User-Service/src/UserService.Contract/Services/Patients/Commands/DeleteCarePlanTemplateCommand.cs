namespace UserService.Contract.Services.Patients.Commands;

public record DeleteCarePlanTemplateCommand : ICommand<Success>
{
    public Guid PatientId { get; init; }
    public Guid Id { get; init; }
}