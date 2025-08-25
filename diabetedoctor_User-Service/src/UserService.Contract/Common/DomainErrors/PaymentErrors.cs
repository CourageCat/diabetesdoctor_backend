using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public class PaymentErrors
{
    public static readonly Error VerifySignatureFailed = Error.Failure(PaymentMessages.VerifySignatureFailed.GetMessage().Code,
        PaymentMessages.VerifySignatureFailed.GetMessage().Message);
}