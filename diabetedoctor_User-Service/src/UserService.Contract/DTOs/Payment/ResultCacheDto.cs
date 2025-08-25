namespace UserService.Contract.DTOs.Payment;

public sealed class ResultCacheDto
{
    public class ServicePackagePaymentCacheDTO
    {
        public ServicePackagePaymentCacheDTO(long orderCode, Guid userId, string description, Guid servicePackageId)
        {
            OrderCode = orderCode;
            UserId = userId;
            Description = description;
            ServicePackageId = servicePackageId;
        }

        public long OrderCode { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public Guid ServicePackageId { get; set; }
    }

}
