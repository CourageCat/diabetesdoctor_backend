namespace UserService.Contract.Services.Doctors.Commands;

public record DeleteCarePlanInstanceCommand : ICommand<Success>
{
    public Guid DoctorId { get; set; }
    public Guid Id { get; set; }
}