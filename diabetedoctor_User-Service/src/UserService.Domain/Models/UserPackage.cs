namespace UserService.Domain.Models;

public class UserPackage : AggregateRoot<Guid>
{
    public string PackageName { get; private set; } = null!;
    public double ConsultationFee { get; private set; }
    public int TotalSessions { get; private set; }
    public int RemainingSessions { get; private set; }
    public DateTime ExpireDate { get; private set; }
    public bool IsExpired { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ServicePackageId { get; private set; }
    public UserInfo User { get; private set; } = null!;
    public ServicePackage ServicePackage { get; private set; } = null!;
    public PaymentHistory PaymentHistory { get; private set; } = null!;

    public UserPackage(Guid id, string packageName, double consultationFee, int totalSessions, int remainingSessions, DateTime expireDate, Guid userId, Guid servicePackageId)
    {
        Id = id;
        PackageName = packageName; 
        ConsultationFee = consultationFee;
        TotalSessions = totalSessions;
        RemainingSessions = remainingSessions;
        ExpireDate = expireDate;
        UserId = userId;
        ServicePackageId = servicePackageId;
    }

    public static UserPackage Create(Guid id, string packageName, double consultationFee, int totalSessions, int remainingSessions, DateTime expireDate, Guid userId, Guid servicePackageId)
    {
        return new UserPackage(id, packageName, consultationFee, totalSessions, remainingSessions, expireDate, userId, servicePackageId);
    }

    public void ReduceSession()
    {
        RemainingSessions--;
    }
    
    public void IncreaseSession()
    {
        RemainingSessions++;
    }
}