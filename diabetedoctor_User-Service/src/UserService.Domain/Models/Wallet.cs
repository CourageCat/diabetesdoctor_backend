namespace UserService.Domain.Models;

public class Wallet : AggregateRoot<Guid>
{
    public double? Revenue { get; private set; }
    public Guid UserId { get; private set; }
    public UserInfo User { get; private set; } = null!;
    private readonly List<WalletTransaction> _walletTransactions = [];
    public IReadOnlyCollection<WalletTransaction> WalletTransactions => _walletTransactions.AsReadOnly();

    public Wallet()
    {
    }

    public Wallet(double? revenue, Guid userId)
    {
        Revenue = revenue;
        UserId = userId;
    }

    public static Wallet Create(double? revenue, Guid userId)
    {
        return new Wallet(revenue, userId);
    }
    
    public void Update(double revenue)
    {
        Revenue += revenue;
    }
}