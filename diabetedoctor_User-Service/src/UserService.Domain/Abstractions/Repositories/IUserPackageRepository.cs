namespace UserService.Domain.Abstractions.Repositories;

public interface IUserPackageRepository : IRepositoryBase<UserPackage, Guid>
{
    Task<UserPackage?> GetFirstActivePackageAsync(Guid userId, CancellationToken cancellationToken);
}