namespace UserService.Contract.Services.Doctors.Commands;

public record DeleteCarePlanTemplateCommand : ICommand<Success>
{ 
    public Guid Id { get; init; }
    public Guid DoctorId { get; init; }
}