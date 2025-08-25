namespace UserService.Contract.Common.Messages;

public enum PaymentMessages
{
    [Message("Thanh toán thành công", "payment_01")]
    CreatePaymentBankingSuccessfully,
    
    [Message("Chữ kí không hợp lệ!", "payment_error_01")]
    VerifySignatureFailed,
}