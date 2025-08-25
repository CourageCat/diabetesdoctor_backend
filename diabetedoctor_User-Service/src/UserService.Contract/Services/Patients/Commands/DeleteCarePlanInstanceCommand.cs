namespace UserService.Contract.Services.Patients.Commands;

public record DeleteCarePlanInstanceCommand : ICommand<Success>
{
    public Guid PatientId { get; set; }
    public Guid Id { get; set; }
}