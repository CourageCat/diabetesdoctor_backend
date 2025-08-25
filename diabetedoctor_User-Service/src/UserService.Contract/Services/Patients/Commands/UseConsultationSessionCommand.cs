namespace UserService.Contract.Services.Patients.Commands;

public record UseConsultationSessionCommand : ICommand<Success>
{
    public Guid UserId  { get; init; }
}