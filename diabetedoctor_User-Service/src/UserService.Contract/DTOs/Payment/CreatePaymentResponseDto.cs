namespace UserService.Contract.DTOs.Payment;

public sealed class CreatePaymentResponseDto
{
    public long OrderCode { get; set; }
    public string Description { get; set; }
    public bool Success { get; set; }
    public string PaymentUrl { get; set; }
    public string Message { get; set; }

    public CreatePaymentResponseDto(long orderCode, string description, bool success, string paymentUrl, string message)
    {
        OrderCode = orderCode;
        Description = description;
        Success = success;
        PaymentUrl = paymentUrl;
        Message = message;
    }
}
