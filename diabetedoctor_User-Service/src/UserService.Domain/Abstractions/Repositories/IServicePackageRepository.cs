namespace UserService.Domain.Abstractions.Repositories;

public interface IServicePackageRepository : IRepositoryBase<ServicePackage, Guid>
{
    Task<double> GetMinimumConsultationFeePerSessionAsync(CancellationToken cancellationToken);
}