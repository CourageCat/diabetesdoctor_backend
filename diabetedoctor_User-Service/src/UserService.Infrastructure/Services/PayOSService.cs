using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using UserService.Application.UseCases.V1.Commands.Payments;
using UserService.Contract.DTOs.Payment;
using UserService.Contract.Infrastructure;

namespace UserService.Infrastructure.Services;

public class PayOSService : IPaymentService
{
    private readonly PayOS _payOS;
    private readonly ILogger<WebhookCommandHandler> _logger;
    public PayOSService(IOptions<PayOSSettings> payOSConfig, ILogger<WebhookCommandHandler> logger)
    {
        _logger = logger;
        _payOS = new PayOS(payOSConfig.Value.ClientId, payOSConfig.Value.ApiKey, payOSConfig.Value.ChecksumKey);
    }

    public async Task<CreatePaymentResponseDto> CreatePaymentLink(CreatePaymentDto paymentData)
    {
        try
        {
            int totalAmount = paymentData.Items.Sum(item => item.Quantity * item.Price);

            List<ItemData> items = paymentData.Items.Select(item =>
                new ItemData(item.ItemName, item.Quantity, item.Price)).ToList();

            PaymentData data = new PaymentData(paymentData.OrderCode, totalAmount, paymentData.Description,
                                               items, paymentData.CancelUrl, paymentData.ReturnUrl);

            CreatePaymentResult createPaymentResult = await _payOS.createPaymentLink(data);

            var responseDTO = new CreatePaymentResponseDto(createPaymentResult.orderCode, createPaymentResult.description, true, createPaymentResult.checkoutUrl, "Success");

            return responseDTO;
        }
        catch (System.Exception exception)
        {
            return null;
        }

    }

    public bool VerifyPayment(WebhookType webhookType)
    {
        try
        {
            _payOS.verifyPaymentWebhookData(webhookType);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error attempting to verify payment webhook: {ExMessage}", ex.Message);
            return false;
        }
    }

    public async Task<bool> CancelOrder(long orderId)
    {
        try
        {
            PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(orderId);
            return true;
        }
        catch (System.Exception exception)
        {
            return false;
        }
    }

}
