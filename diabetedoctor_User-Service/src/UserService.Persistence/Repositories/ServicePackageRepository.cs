namespace UserService.Persistence.Repositories;

public class ServicePackageRepository(ApplicationDbContext context) : RepositoryBase<ServicePackage, Guid>(context),  IServicePackageRepository
{
    public async Task<double> GetMinimumConsultationFeePerSessionAsync(CancellationToken cancellationToken)
    {
        return await context.ServicePackages
            .Where(sp => sp.Sessions > 0)
            .Select(sp => sp.Price / sp.Sessions)
            .MinAsync(cancellationToken);
    }
}