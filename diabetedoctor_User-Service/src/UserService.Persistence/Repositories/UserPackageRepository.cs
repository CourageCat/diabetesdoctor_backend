namespace UserService.Persistence.Repositories;

public class UserPackageRepository(ApplicationDbContext context) : RepositoryBase<UserPackage, Guid>(context), IUserPackageRepository
{
    public async Task<UserPackage?> GetFirstActivePackageAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.UserPackages
            .Where(up => !up.IsExpired && up.RemainingSessions > 0 && up.UserId == userId)
            .OrderBy(up => up.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}