namespace UserService.Domain.Models;

public class PaymentHistory : AggregateRoot<Guid>
{
    public double Amount { get; private set; }
    public long OrderCode { get; private set; }
    public Guid UserId { get; private set; }
    public Guid UserPackageId { get; private set; }
    public UserInfo User { get; private set; } = null!;
    public UserPackage UserPackage { get; private set; } = null!;

    public PaymentHistory(Guid id, double amount, long orderCode, Guid userId, Guid userPackageId)
    {
        Id = id;
        Amount = amount;
        OrderCode = orderCode;
        UserId = userId;
        UserPackageId = userPackageId;
    }

    public static PaymentHistory Create(Guid id, double amount, long orderCode, Guid userId, Guid userPackageId)
    {
        return new PaymentHistory(id, amount, orderCode, userId, userPackageId);
    }
}