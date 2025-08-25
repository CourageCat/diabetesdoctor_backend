namespace UserService.Domain.Models;

public class WalletTransaction : AggregateRoot<Guid>
{
    public Guid WalletId { get; private set; }
    public Wallet Wallet { get; private set; } = null!;

    /// <summary>
    /// Số tiền giao dịch (luôn >= 0)
    /// </summary>
    public double Amount { get; private set; }

    /// <summary>
    /// Dòng tiền: In (cộng vào) / Out (trừ ra)
    /// </summary>
    public TransactionDirection Direction { get; private set; }

    /// <summary>
    /// Số dư sau khi thực hiện giao dịch
    /// </summary>
    public double BalanceAfterTransaction { get; private set; }

    /// <summary>
    /// Loại giao dịch: Nạp, Rút, Mua hàng, Hoàn tiền, Khuyến mãi, Tư vấn riêng...
    /// </summary>
    public TransactionType Type { get; private set; }

    /// <summary>
    /// Mô tả chi tiết (tùy chọn)
    /// </summary>
    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private WalletTransaction() { }

    public WalletTransaction(
        Guid walletId,
        double amount,
        TransactionDirection direction,
        double balanceAfterTransaction,
        TransactionType type,
        string? description)
    {
        WalletId = walletId;
        Amount = amount;
        Direction = direction;
        BalanceAfterTransaction = balanceAfterTransaction;
        Type = type;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }
}