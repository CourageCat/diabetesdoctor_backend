namespace UserService.Contract.Services.Patients.Commands;

public record RefundConsultationSessionCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid UserPackageId {get; init;}
}