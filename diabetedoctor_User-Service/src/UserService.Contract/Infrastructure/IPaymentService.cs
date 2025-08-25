using Net.payOS.Types;
using UserService.Contract.DTOs.Payment;

namespace UserService.Contract.Infrastructure;

public interface IPaymentService
{
    Task<CreatePaymentResponseDto> CreatePaymentLink(CreatePaymentDto paymentData);
    bool VerifyPayment(WebhookType webhookType);
    Task<bool> CancelOrder(long orderId);
}
