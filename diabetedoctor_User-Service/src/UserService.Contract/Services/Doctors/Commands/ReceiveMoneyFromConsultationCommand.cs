namespace UserService.Contract.Services.Doctors.Commands;

public record ReceiveMoneyFromConsultationCommand : ICommand
{
    public Guid UserId { get; init; }
    public double ConsultationFee { get; init; }    
}