namespace UserService.Contract.Services.ServicePackages.Responses;

public record ServicePackagePurchasedResponse
{
    public string Id { get; init; } = string.Empty;
    public string PackageName { get; init; } = string.Empty;
    public double PriceAtPurchased { get; init; }
    public int TotalSessions { get; init; }
    public int RemainingSessions { get; init; }
    public DateTime ExpireDate { get; init; }
    public bool IsExpired { get; init; }
    public DateTime PurchasedDate { get; init; }
}