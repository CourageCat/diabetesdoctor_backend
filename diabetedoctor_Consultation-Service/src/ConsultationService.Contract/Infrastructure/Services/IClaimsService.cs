namespace ConsultationService.Contract.Infrastructure.Services;

public interface IClaimsService
{
    public string GetCurrentUserId { get; }
    public string GetCurrentRole { get; }
}