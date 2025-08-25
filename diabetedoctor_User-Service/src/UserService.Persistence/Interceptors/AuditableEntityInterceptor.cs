using UserService.Contract.Infrastructure;

namespace UserService.Persistence.Interceptors;
public sealed class AuditableEntityInterceptor(IUserContext userContext)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;
        var date = DateTime.UtcNow;
        var userId = userContext.UserId;
        foreach (var entry in context.ChangeTracker.Entries<IDomainEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = date;
                entry.Entity.CreatedBy = userId;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.ModifiedDate = date;
                entry.Entity.UpdatedBy = userId;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
