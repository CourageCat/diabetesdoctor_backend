using UserService.Contract.DTOs.Payment;

namespace UserService.Contract.Services.Payments.Commands;

public record CreatePaymentBankingCommand : ICommand<Success<CreatePaymentResponseDto>>
{
    public Guid UserId { get; init; }
    public Guid ServicePackageId  { get; init; }
}